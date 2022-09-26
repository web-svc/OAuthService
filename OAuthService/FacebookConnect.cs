namespace OAuthService
{
    using CryptoService;
    using CryptoService.Interface;
    using CryptoService.Model;
    using Helper;
    using Helper.Extentsion;
    using Helper.Interface;
    using Helper.Model;
    using Newtonsoft.Json;
    using OAuthService.Interface;
    using OAuthService.Interface.Facebook;
    using OAuthService.Model;
    using OAuthService.Model.Facebook;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using static OAuthService.Constant.Const;
    using static OAuthService.Constant.Enum;

    public class FacebookConnect
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private static string _callBackUrl;
        private string _scope;
        private string _authorizeUrl;
        private string _accessTockenUrl;
        private string _userJsonUrl;
        static NameValueCollection QueryString = HttpUtility.ParseQueryString(string.Empty);
        readonly ITextService textService = new TextService();
        readonly IUrlService urlService = new UrlService();
        readonly IAuthService authService = new AuthService();

        public FacebookConnect(string consumerKey, string consumerSecret, string callbackUrl, string scope = "public_profile,email", string authorizeUrl = "https://www.facebook.com/v2.9/dialog/oauth", string accessTockenUrl = "https://graph.facebook.com/v2.9/oauth/access_token", string userJsonUrl = "https://graph.facebook.com/me?fields=email,name,first_name,last_name,link")
        {
            _apiKey = consumerKey;
            _apiSecret = consumerSecret;
            _scope = scope;
            _authorizeUrl = authorizeUrl;
            _accessTockenUrl = accessTockenUrl;
            _userJsonUrl = userJsonUrl;
            _callBackUrl = callbackUrl;
            QueryString = HttpUtility.ParseQueryString(new Uri(_callBackUrl).Query);
        }
        
        public static bool IsAuthorized => !QueryString[AuthCode].IsEmpty();
        
        public static bool IsDenied => !QueryString[ErrorCode].IsEmpty();
        
        public string GetAuthenticationUrl()
        {
            var uriBuilder = new UriBuilder(_authorizeUrl);
            var ListQueryString = urlService.ParseQueryString(uriBuilder.Query);
            var form = new Dictionary<string, string>
            {
                {Scope, urlService.UrlEncode(_scope)},
                {CsrfState, textService.GenerateNonce()},
                {RedirectUri, _callBackUrl},
                {ResponseType, ResponseTypeValue},
                {ClientId, _apiKey},
            };
            ListQueryString.AddRange(form.Select(x => new Parameters() { Name = x.Key, Value = x.Value }).ToList());
            uriBuilder.Query = urlService.QueryBuilder(ListQueryString);

            return uriBuilder.Uri.AbsoluteUri;
        }

        public IFacebookToken Authorize(string CsrfState)
        {
            ITokenRequest tokenRequest = new TokenRequest()
            {
                ClientId = _apiKey,
                ClientSecret = _apiSecret,
                Code = QueryString[AuthCode],
                GrantType = GrantTypeValue,
                RedirectUri = _callBackUrl.Replace(new Uri(_callBackUrl).Query, string.Empty),
                BaseUri = _accessTockenUrl,
                BasicXAuthCode = authService.GenerateBasicAuthWithHeader(new BasicAuthInput() { UserName = _apiKey, Password = _apiSecret }),
                CsrfState = CsrfState,
                QueryString = QueryString,
            };
            IFacebookToken token = JsonConvert.DeserializeObject<FacebookToken>(Service.GetAccessTokenJson(tokenRequest));
            return token;
        }

        public IFacebookUser GetUser(IFacebookToken token)
        {
            IFacebookUser user = JsonConvert.DeserializeObject<FacebookUser>(GetUserJson(token));
            user.AuthRouter = nameof(AuthRouter.Facebook);
            user.AccessToken = token.AccessToken;
            return user;
        }

        public string GetUserJson(IFacebookToken token)
        {
            IHttpResponseInput httpResponseInput = new HttpResponseInput()
            {
                BaseUri = _userJsonUrl,
                XAuthHeader = authService.ConstructBearerAuth(token.AccessToken),
            };

            return Service.GetUserJson(token, httpResponseInput);
        }
    }
}