namespace OAuthService
{
    using CryptoService;
    using CryptoService.Interface;
    using Helper;
    using Helper.Extentsion;
    using Helper.Interface;
    using Helper.Model;
    using Newtonsoft.Json;
    using OAuthService.Interface;
    using OAuthService.Interface.Twitter;
    using OAuthService.Model;
    using OAuthService.Model.Twitter;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using static OAuthService.Constant.Const;
    using static OAuthService.Constant.Enum;

    public class TwitterConnectV1
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private string _callBackUrl;
        private string _resourceUrl;
        private string _authorizeUrl;
        private string _accessTockenUrl;
        private string _userJsonUrl;
        static NameValueCollection QueryString = HttpUtility.ParseQueryString(string.Empty);
        readonly ITextService textService = new TextService();
        readonly IUrlService urlService = new UrlService();
        readonly IAuthService authService = new AuthService();
        public TwitterConnectV1(string consumerKey, string consumerSecret, string callback, string resourceUrl = "https://api.twitter.com/oauth/authorize", string authorizeUrl = "https://api.twitter.com/oauth/request_token", string accessTockenUrl = "https://api.twitter.com/oauth/access_token", string userJsonUrl = "https://api.twitter.com/1.1/account/verify_credentials.json")
        {
            _apiKey = consumerKey;
            _apiSecret = consumerSecret;
            _resourceUrl = resourceUrl;
            _authorizeUrl = authorizeUrl;
            _accessTockenUrl = accessTockenUrl;
            _userJsonUrl = userJsonUrl;
            _callBackUrl = callback;
            QueryString = HttpUtility.ParseQueryString(new Uri(_callBackUrl).Query);
        }
        public static bool IsAuthorizedAndValidate => !QueryString[OAuthToken].IsEmpty();
        public static bool IsDenied => !QueryString[Denied].IsEmpty();
        public string GetAuthenticationUrl(ITwitterOAuthToken OAuthRequestToken)
        {
            OAuthRequestToken.ThrowIfNull();
            var uriBuilder = new UriBuilder(_authorizeUrl);
            var ListQueryString = urlService.ParseQueryString(uriBuilder.Query);
            if (OAuthRequestToken.OAuthCallbackConfirmed && !OAuthRequestToken.AccessToken.IsEmpty())
            {
                ListQueryString.Add(new Parameters() { Name = OAuthToken, Value = OAuthRequestToken.AccessToken });
                uriBuilder = new UriBuilder(_resourceUrl)
                {
                    Query = urlService.QueryBuilder(ListQueryString)
                };
            }

            return uriBuilder.Uri.AbsoluteUri;
        }
        public ITwitterOAuthToken RequestOAuthToken()
        {
            var uriBuilder = new UriBuilder(_authorizeUrl);
            var ListQueryString = urlService.ParseQueryString(uriBuilder.Query);
            var compositeKey = string.Concat(Uri.EscapeDataString(_apiSecret), "&", Uri.EscapeDataString(string.Empty));

            var form = new Dictionary<string, string>
            {
                {OAuthCallback, urlService.UrlEncode(_callBackUrl)},
                {OAuthConsumerKey, _apiKey},
                {OAuthNonce, textService.GenerateNonce()},
                {OAuthSignatureMethod, OAuthSignatureMethodValue},
                {OAuthTimeStamp, textService.GenerateTimeStamp()},
                {OAuthVersion, OAuthVersionValue}
            };

            ListQueryString.AddRange(form.Select(x => new Parameters() { Name = x.Key, Value = x.Value }).ToList());

            string hashValue = authService.GetOAuthSignature(compositeKey, ListQueryString.ToDictionary(x => x.Name, x => x.Value), _authorizeUrl);

            ListQueryString.Add(new Parameters() { Name = OAuthSignature, Value = urlService.UrlEncode(hashValue) });
            uriBuilder.Query = urlService.QueryBuilder(ListQueryString);

            var OAuthTokenJson = Service.RequestOAuthTokenJson(uriBuilder.Uri.AbsoluteUri);
            ITwitterOAuthToken OAuthToken = JsonConvert.DeserializeObject<TwitterOAuthToken>(OAuthTokenJson);
            return OAuthToken;
        }
        public ITwitterOAuthToken Authorize()
        {
            var uriBuilder = new UriBuilder(_accessTockenUrl);

            var ListQueryString = urlService.ParseQueryString(uriBuilder.Query);
            var compositeKey = string.Concat(Uri.EscapeDataString(_apiSecret), "&", Uri.EscapeDataString(string.Empty));

            var form = new Dictionary<string, string>
            {
                {OAuthCallback, urlService.UrlEncode(_callBackUrl)},
                {OAuthConsumerKey, _apiKey},
                {OAuthNonce, textService.GenerateNonce()},
                {OAuthSignatureMethod, OAuthSignatureMethodValue},
                {OAuthTimeStamp, textService.GenerateTimeStamp()},
                {OAuthVersion, OAuthVersionValue},
                {OAuthToken, QueryString[OAuthToken]},
                {OAuthVerifier, QueryString[OAuthVerifier]}
            };
            ListQueryString.AddRange(form.Select(x => new Parameters() { Name = x.Key, Value = x.Value }).ToList());

            string hashValue = authService.GetOAuthSignature(compositeKey, ListQueryString.ToDictionary(x => x.Name, x => x.Value), _authorizeUrl);
            
            ListQueryString.Add(new Parameters() { Name = OAuthSignature, Value = urlService.UrlEncode(hashValue) });

            uriBuilder.Query = urlService.QueryBuilder(ListQueryString);

            var OAuthTokenJson = Service.RequestOAuthTokenJson(uriBuilder.Uri.AbsoluteUri);
            return JsonConvert.DeserializeObject<TwitterOAuthToken>(OAuthTokenJson);
        }
        public IUser GetUser(ITwitterOAuthToken token)
        {
            var userJson = GetUserJson(token);
            var user = JsonConvert.DeserializeObject<TwitterUser>(userJson);
            user.AccessToken = token.AccessToken;
            user.AuthRouter = nameof(AuthRouter.Twitter);
            return user;
        }
        public string GetUserJson(ITwitterOAuthToken token)
        {
            var uriBuilder = new UriBuilder(_userJsonUrl);
            var ListQueryString = urlService.ParseQueryString(uriBuilder.Query);
            ListQueryString.Add(new Parameters() { Name = IncludeEmail, Value = true.ToString().ToLower() });
            var form = new Dictionary<string, string>
            {
                    {OAuthConsumerKey, _apiKey},
                    {OAuthNonce, textService.GenerateNonce()},
                    {OAuthSignatureMethod, OAuthSignatureMethodValue},
                    {OAuthTimeStamp, textService.GenerateTimeStamp()},
                    {OAuthToken, token.AccessToken },
                    {OAuthVersion, OAuthVersionValue}
            };
            var compositeKey = string.Concat(Uri.EscapeDataString(_apiSecret), "&", Uri.EscapeDataString(token.OAuthTokenSecret));

            form.Add(OAuthSignature, authService.GetOAuthSignature(compositeKey, form, _userJsonUrl, Uri.EscapeDataString(urlService.QueryBuilder(ListQueryString))));

            uriBuilder.Query = urlService.QueryBuilder(ListQueryString);

            IHttpResponseInput httpResponseInput = new HttpResponseInput()
            {
                BaseUri = uriBuilder.Uri.AbsoluteUri,
                XAuthHeader = authService.ConstructOAuth(form),
            };

            var userJson = Service.GetUserJson(token, httpResponseInput);
            return userJson;
        }    
    }
}