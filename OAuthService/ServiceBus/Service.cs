namespace OAuthService
{
    using Helper;
    using Newtonsoft.Json;
    using OAuthService.Interface;
    using OAuthService.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using static OAuthService.Constant.Const;

    internal class Service
    {
        internal static async Task<HttpResponseMessage> PostHttpResponseMessageAsync(IHttpResponseInput httpResponseInput)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            using HttpClient client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add(AuthHeaderKey, httpResponseInput.XAuthHeader);
            HttpResponseMessage httpResponseMessage = await client.PostAsync(httpResponseInput.BaseUri, new FormUrlEncodedContent(httpResponseInput.FormField));
            return httpResponseMessage;
        }

        internal static async Task<HttpResponseMessage> GetHttpResponseMessageAsync(IHttpResponseInput httpResponseInput)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            using HttpClient client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add(AuthHeaderKey, httpResponseInput.XAuthHeader);
            HttpResponseMessage httpResponseMessage = await client.GetAsync(httpResponseInput.BaseUri);
            return httpResponseMessage;
        }

        internal static string GetUserJson(IToken token, IHttpResponseInput httpResponseInput)
        {
            ExceptionHandler.ThrowIfNull(token);
            ExceptionHandler.ThrowIfNullOrEmpty(token.AccessToken);

            var task = Task.Run(() => GetHttpResponseMessageAsync(httpResponseInput)).ConfigureAwait(false);
            var tokenResponse = task.GetAwaiter().GetResult();

            var ReadContent = Task.Run(() => tokenResponse.Content.ReadAsStringAsync()).ConfigureAwait(false);
            var jsonContent = ReadContent.GetAwaiter().GetResult();            

            return jsonContent;
        }

        internal static HttpResponseMessage GetAccessToken(ITokenRequest tokenRequest)
        {
            tokenRequest.CsrfState.ThrowIfNull();
            if (tokenRequest.QueryString[CsrfState] != tokenRequest.CsrfState)
                ExceptionHandler.ThrowUnauthorizedAccess(InvalidCsrf);
            if (tokenRequest.QueryString[AuthCode]?.Length <= 0)
                ExceptionHandler.ThrowUnauthorizedAccess(InvalidToken);

            var form = new Dictionary<string, string>
            {
                {ClientId, tokenRequest.ClientId},
                {RedirectUri, tokenRequest.RedirectUri},
                {ClientSecret, tokenRequest.ClientSecret},
                {AuthCode, tokenRequest.Code},
                {GrantType, tokenRequest.GrantType},
            };

            IHttpResponseInput httpResponseInput = new HttpResponseInput()
            {
                BaseUri = tokenRequest.BaseUri,
                XAuthHeader = tokenRequest.BasicXAuthCode,
                FormField = form,
            };

            var task = Task.Run(() => PostHttpResponseMessageAsync(httpResponseInput)).ConfigureAwait(false);
            HttpResponseMessage tokenResponse = task.GetAwaiter().GetResult();
            return tokenResponse;
        }

        internal static HttpResponseMessage GetAccessTokenTwitter(ITokenRequest tokenRequest)
        {
            tokenRequest.CsrfState.ThrowIfNull();
            if (tokenRequest.QueryString[CsrfState] != tokenRequest.CsrfState)
                ExceptionHandler.ThrowUnauthorizedAccess(InvalidCsrf);
            if (tokenRequest.QueryString[AuthCode]?.Length <= 0)
                ExceptionHandler.ThrowUnauthorizedAccess(InvalidToken);

            var form = new Dictionary<string, string>
                {
                    {ClientId, tokenRequest.ClientId},
                    {RedirectUri, tokenRequest.RedirectUri},
                    {CodeVerifier, CodeChallengeValue},
                    {AuthCode, tokenRequest.Code},
                    {GrantType, tokenRequest.GrantType},
                };

            IHttpResponseInput httpResponseInput = new HttpResponseInput()
            {
                BaseUri = tokenRequest.BaseUri,
                XAuthHeader = tokenRequest.BasicXAuthCode,
                FormField = form,
            };

            var task = Task.Run(() => PostHttpResponseMessageAsync(httpResponseInput)).ConfigureAwait(false);
            HttpResponseMessage tokenResponse = task.GetAwaiter().GetResult();
            return tokenResponse;
        }

        internal static string GetAccessTokenJson(ITokenRequest tokenRequest)
        {
            tokenRequest.CsrfState.ThrowIfNull();
            if (tokenRequest.QueryString[CsrfState] != tokenRequest.CsrfState)
                ExceptionHandler.ThrowUnauthorizedAccess(InvalidCsrf);
            if (tokenRequest.QueryString[AuthCode]?.Length <= 0)
                ExceptionHandler.ThrowUnauthorizedAccess(InvalidToken);

            var form = new Dictionary<string, string>
            {
                {ClientId, tokenRequest.ClientId},
                {RedirectUri, tokenRequest.RedirectUri},
                {ClientSecret, tokenRequest.ClientSecret},
                {AuthCode, tokenRequest.Code},
                {GrantType, tokenRequest.GrantType},
            };

            IHttpResponseInput httpResponseInput = new HttpResponseInput()
            {
                BaseUri = tokenRequest.BaseUri,
                XAuthHeader = tokenRequest.BasicXAuthCode,
                FormField = form,
            };

            var task = Task.Run(() => PostHttpResponseMessageAsync(httpResponseInput)).ConfigureAwait(false);
            HttpResponseMessage tokenResponse = task.GetAwaiter().GetResult();
            var ReadContent = Task.Run(() => tokenResponse.Content.ReadAsStringAsync()).ConfigureAwait(false);
            var jsonContent = ReadContent.GetAwaiter().GetResult();
            return jsonContent;
        }

        internal static string RequestOAuthTokenJson(string BaseUri)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            using HttpClient client = new HttpClient(httpClientHandler);

            var task  = client.GetAsync(BaseUri).ConfigureAwait(false);
            HttpResponseMessage tokenResponse = task.GetAwaiter().GetResult();
            var ReadContent = Task.Run(() => tokenResponse.Content.ReadAsStringAsync()).ConfigureAwait(false);
            var jsonContent = ReadContent.GetAwaiter().GetResult();

            var OAuthTokenQueryString = HttpUtility.ParseQueryString(jsonContent);
            if(tokenResponse.StatusCode == HttpStatusCode.OK)
                return JsonConvert.SerializeObject(OAuthTokenQueryString?.AllKeys?.ToDictionary(x => x, x => OAuthTokenQueryString[x]));
            return jsonContent;
        }
    }
}
