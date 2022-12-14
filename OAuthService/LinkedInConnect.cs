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
    using OAuthService.Interface.LinkedIn;
    using OAuthService.Model;
    using OAuthService.Model.LinkedIn;
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

    public class LinkedInConnect
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private static string _callBackUrl;
        private string _scope;
        private string _authorizeUrl;
        private string _accessTockenUrl;
        private List<string> _userJsonUrl;
        static NameValueCollection QueryString = HttpUtility.ParseQueryString(string.Empty);
        readonly ITextService textService = new TextService();
        readonly IUrlService urlService = new UrlService();
        readonly IAuthService authService = new AuthService();

        public LinkedInConnect(string consumerKey, string consumerSecret, string callbackUrl, string scope = "r_liteprofile r_emailaddress", string authorizeUrl = "https://www.linkedin.com/oauth/v2/authorization", string accessTockenUrl = "https://www.linkedin.com/oauth/v2/accessToken", string userJsonUrl = "https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~)) https://api.linkedin.com/v2/me https://api.linkedin.com/v2/me?projection=(id,firstName,lastName,profilePicture(displayImage~:playableStreams))")
        {
            _apiKey = consumerKey;
            _apiSecret = consumerSecret;
            _scope = scope;
            _authorizeUrl = authorizeUrl;
            _accessTockenUrl = accessTockenUrl;
            _userJsonUrl = userJsonUrl.Split(' ').ToList();
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

        public ILinkedInToken Authorize(string CsrfState)
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
            ILinkedInToken token = JsonConvert.DeserializeObject<LinkedInToken>(jsonContent);
            return token;
        }

        public ILinkedInUser GetUser(IToken token)
        {
            ILinkedInUser user = JsonConvert.DeserializeObject<LinkedInUser>(GetUserJson(token));
            
            user.AuthRouter = nameof(AuthRouter.LinkedIn);
            user.AccessToken = token.AccessToken;
            user.Name = $"{user?.FirstName} {user?.LastName}";
            user.Email = $"{user.Elements?.FirstOrDefault().Handle?.Email}";
            return user;
        }

        public string GetUserJson(IToken token)
        {
            var JsonObject = new JObject();
            foreach (var BaseUri in _userJsonUrl)
            {
                IHttpResponseInput httpResponseInput = new HttpResponseInput()
                {
                    BaseUri = BaseUri,
                    XAuthHeader = authService.ConstructBearerAuth(token.AccessToken),
                };

                var json = Service.GetUserJson(token, httpResponseInput);
                JsonObject.Merge(JObject.Parse(json));
            }
            return JsonObject.ConvertToString();
        }
    }
}