using Newtonsoft.Json;
using OAuthService.Constant;
using OAuthService.Extentsion;
using OAuthService.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace OAuthService
{
    public class FacebookConnect
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private string _callBackUrl;
        private string _scope;
        private string _authorizeUrl;
        private string _accessTockenUrl;
        private string _userJsonUrl;
        static NameValueCollection QueryString = HttpUtility.ParseQueryString(string.Empty);
        public FacebookConnect(string consumerKey, string consumerSecret, string scope = "public_profile,email", string authorizeUrl = "https://www.facebook.com/v2.9/dialog/oauth", string accessTockenUrl = "https://graph.facebook.com/v2.9/oauth/access_token", string userJsonUrl = "https://graph.facebook.com/me?fields=email,name,first_name,last_name,link")
        {
            _apiKey = consumerKey;
            _apiSecret = consumerSecret;
            _scope = scope;
            _authorizeUrl = authorizeUrl;
            _accessTockenUrl = accessTockenUrl;
            _userJsonUrl = userJsonUrl;
        }
        public static bool IsAuthorized => !QueryString["code"].IsEmpty();
        public static bool IsDenied => !QueryString["error"].IsEmpty();
        public void Authentication(string callBackUrl)
        {
            _callBackUrl = callBackUrl; //HttpContext.Current.Response.Redirect(AuthenticationUrl);
        }
        private string AuthenticationUrl
        {
            get
            {
                var uriBuilder = new UriBuilder(_authorizeUrl);
                var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
                App.XOAUTH_Guid = GenerateNonce();
                //HttpContext.Current.Session.Add("csrf_state", App.XOAUTH_Guid);
                var queryParameters = GetQueryParameters(uriBuilder.Query);
                queryParameters.Add(new QueryParameter("client_id", _apiKey));
                queryParameters.Add(new QueryParameter("redirect_uri", _callBackUrl));
                queryParameters.Add(new QueryParameter("scope", UrlEncode(_scope)));
                queryParameters.Add(new QueryParameter("state", App.XOAUTH_Guid));
                queryParameters.Add(new QueryParameter("response_type", Const.ResponseType));
                uriBuilder.Query = NormalizeRequestParameters(queryParameters);
                return uriBuilder.Uri.AbsoluteUri;
            }
        }
        public void Authorize(string callBackUrl)
        {
            _callBackUrl = callBackUrl; AccessTokenGet(QueryString["code"], QueryString["state"]);
            if (App.TokenSecret.Length <= 0)
                throw new Exception("Invalid Facebook token.");
        }
        private void AccessTokenGet(string authToken, string oauthVerifier)
        {
            App.Token = authToken;
            App.OAuthVerifier = oauthVerifier;
            //if (HttpContext.Current.Session["csrf_state"] == null || !HttpContext.Current.Session["csrf_state"].Equals(oauthVerifier))
            //    throw new Exception("auth data data has been tempered but you are safe.");
            var uriBuilder = new UriBuilder(_accessTockenUrl);
            var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
            var queryParameters = GetQueryParameters(uriBuilder.Query);
            queryParameters.Add(new QueryParameter("client_id", _apiKey));
            queryParameters.Add(new QueryParameter("redirect_uri", UrlEncode(_callBackUrl)));
            queryParameters.Add(new QueryParameter("client_secret", _apiSecret));
            queryParameters.Add(new QueryParameter("code", UrlEncode(App.Token)));
            queryParameters.Add(new QueryParameter("grant_type", Const.GrantType));
            var query = NormalizeRequestParameters(queryParameters);
            App.TokenSecret = JsonConvert.DeserializeObject<dynamic>(WebRequest(Method.POST, uriBuilder.Uri.AbsoluteUri, query))["access_token"].ToString();
        }
        private string WebRequest(Method method, string url, string postData)
        {
            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (webRequest == null) return null;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            if (method == Method.POST || method == Method.DELETE)
            {
                webRequest.Headers.Add(AuthorizationHeader);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var streamWriter2 = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    streamWriter2.Write(postData);
                }
                finally
                {
                    streamWriter2.Close();
                }
            }
            var str = WebResponseGet(webRequest);
            return str;
        }
        private string AuthorizationHeader
        {
            get
            {
                return "Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_apiKey + ":" + _apiSecret));
            }
        }
        private string AuthorizedHeader
        {
            get
            {
                return "Authorization: Bearer " + App.TokenSecret;
            }
        }
        public User FetchProfile()
        {
            var userJson = JsonConvert.DeserializeObject<dynamic>(GetAuthRequest(Method.GET, _userJsonUrl, AuthorizedHeader));
            if (userJson == null) { throw new Exception("? OAuth Token not received. Please retry or get in touch with support team."); }
            if (userJson.ContainsKey("email") == false) { throw new Exception("we could not fetch your email address from OAuth Token. Please make sure that your profile is up to date with email address."); }
            return new User { Email = userJson["email"], Name = userJson["name"], Token = App.TokenSecret, AuthRouter = "Facebook" };
        }
        private static string GetAuthRequest(Method method, string url, string postData)
        {
            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (webRequest == null) return null;
            webRequest.Method = method.ToString();
            if (postData.Length > 0)
            {
                webRequest.Headers.Add(postData);
                webRequest.KeepAlive = true;
            }
            var str = WebResponseGet(webRequest);
            return str;
        }
        private static string WebResponseGet(HttpWebRequest webRequest)
        {
            var stream = webRequest.GetResponse().GetResponseStream();
            return stream == null ? null : new StreamReader(stream).ReadToEnd();
        }
        private enum Method
        {
            GET,
            POST,
            DELETE,
        }
        private static string UrlEncode(string value)
        {
            var stringBuilder = new StringBuilder();
            foreach (var ch in value)
            {
                if (Const.UnreservedChars.IndexOf(ch) != -1)
                    stringBuilder.Append(ch);
                else
                    stringBuilder.Append('%' + $"{(int)ch:X2}");
            }
            return stringBuilder.ToString();
        }
        private static string GenerateNonce()
        {
            return new Random().Next(123400, 9999999).ToString();
        }
        private static string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < parameters.Count; ++index)
            {
                var parameter = parameters[index];
                stringBuilder.AppendFormat("{0}={1}", parameter.Name, parameter.Value);
                if (index < parameters.Count - 1)
                    stringBuilder.Append("&");
            }
            return stringBuilder.ToString();
        }
        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
                parameters = parameters.Remove(0, 1);
            var queryParameterList = new List<QueryParameter>();
            if (string.IsNullOrEmpty(parameters)) return queryParameterList;
            var str = parameters;
            var chArray = new[] { '&' };
            foreach (var name in str.Split(chArray))
            {
                if (string.IsNullOrEmpty(name) || name.StartsWith("oauth_")) continue;
                if (name.IndexOf('=') > -1)
                {
                    var strArray = name.Split('=');
                    queryParameterList.Add(new QueryParameter(strArray[0], strArray[1]));
                }
                else
                    queryParameterList.Add(new QueryParameter(name, string.Empty));
            }
            return queryParameterList;
        }
        private class QueryParameter
        {
            public string Name { get; }

            public string Value { get; }

            public QueryParameter(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}