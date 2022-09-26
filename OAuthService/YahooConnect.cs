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
    using Newtonsoft.Json.Linq;
    using OAuthService.Interface;
    using OAuthService.Interface.Yahoo;
    using OAuthService.Model;
    using OAuthService.Model.Yahoo;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using static OAuthService.Constant.Const;
    using static OAuthService.Constant.Enum;

    public class YahooConnect
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private static string _callBackUrl;
        private readonly string _scope;
        private string _authorizeUrl;
        private string _accessTockenUrl;
        private string _userJsonUrl;
        static NameValueCollection QueryString = HttpUtility.ParseQueryString(string.Empty);
        readonly ITextService textService = new TextService();
        readonly IUrlService urlService = new UrlService();
        readonly IAuthService authService = new AuthService();

        public YahooConnect(string consumerKey, string consumerSecret, string callbackUrl, string scope = "openid mail-r", string authorizeUrl = "https://api.login.yahoo.com/oauth2/request_auth", string accessTockenUrl = "https://api.login.yahoo.com/oauth2/get_token", string userJsonUrl = "https://api.login.yahoo.com/openid/v1/userinfo")
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
        
        public static bool IsDenied => !QueryString[Error].IsEmpty();
        
        public string GetAuthenticationUrl()
        {
            var uriBuilder = new UriBuilder(_authorizeUrl);
            var ListQueryString = urlService.ParseQueryString(uriBuilder.Query);

            var form = new Dictionary<string, string>
            {
                {CsrfState, textService.GenerateNonce()},
                {RedirectUri, urlService.UrlEncode(_callBackUrl)},
                {ResponseType, ResponseTypeValue},
                {ClientId, _apiKey},
            };
            ListQueryString.AddRange(form.Select(x => new Parameters() { Name = x.Key, Value = x.Value }).ToList());
            uriBuilder.Query = urlService.QueryBuilder(ListQueryString);

            return uriBuilder.Uri.AbsoluteUri;
        }

        public IYahooToken Authorize(string CsrfState)
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
            
            HttpResponseMessage tokenResponse = Service.GetAccessToken(tokenRequest);

            var ReadContent = Task.Run(() => tokenResponse.Content.ReadAsStringAsync()).ConfigureAwait(false);
            var jsonContent = ReadContent.GetAwaiter().GetResult();
            if (tokenResponse.StatusCode != HttpStatusCode.OK)
            {
                var JsonObj = new JObject
                {
                    { nameof(Error), JObject.Parse(jsonContent) }
                };
                jsonContent = JsonObj.ToString();
            }
            IYahooToken token = JsonConvert.DeserializeObject<YahooToken>(jsonContent);
            return token;
        }

        public IYahooUser GetUser(IToken token)
        {
            IYahooUser user = JsonConvert.DeserializeObject<YahooUser>(GetUserJson(token));
            user.AuthRouter = nameof(AuthRouter.Yahoo);
            user.AccessToken = token.AccessToken;
            return user;
        }

        public string GetUserJson(IToken token)
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