using Newtonsoft.Json;
using OAuthService.Constant;
using OAuthService.Extentsion;
using OAuthService.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OAuthService
{
    public class TwitterConnect
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private string _callBackUrl;
        private string _resourceUrl;
        private string _authorizeUrl;
        private string _accessTockenUrl;
        private string _userJsonUrl;
        static NameValueCollection QueryString = HttpUtility.ParseQueryString(string.Empty);
        public TwitterConnect(string consumerKey, string consumerSecret, string resourceUrl = "https://api.twitter.com/oauth/authorize?oauth_token=", string authorizeUrl = "https://api.twitter.com/oauth/request_token", string accessTockenUrl = "https://api.twitter.com/oauth/access_token", string userJsonUrl = "https://api.twitter.com/1.1/account/verify_credentials.json?include_entities=false&skip_status=true&include_email=true")
        {
            _apiKey = consumerKey;
            _apiSecret = consumerSecret;
            _resourceUrl = resourceUrl;
            _authorizeUrl = authorizeUrl;
            _accessTockenUrl = accessTockenUrl;
            _userJsonUrl = userJsonUrl;
        }
        public static bool IsAuthorized => !QueryString["oauth_token"].IsEmpty();
        public static bool IsDenied => !QueryString["denied"].IsEmpty();
        public void Authorize(string callBackUrl)
        {
            _callBackUrl = callBackUrl; //HttpContext.Current.Response.Redirect(GetAutorizedUrl());
        }
        public User FetchProfile()
        {
            AccessTokenGet(QueryString["oauth_token"], QueryString["oauth_verifier"]);
            if (App.TokenSecret.Length <= 0)
                throw new Exception("Invalid Twitter token.");
            var userJson = JsonConvert.DeserializeObject<dynamic>(GetoAuthRequest(Method.GET, _userJsonUrl, string.Empty));
            if (userJson == null) { throw new Exception("? OAuth Token not received. Please retry or get in touch with support team."); }
            if (userJson.ContainsKey("email") == false) { throw new Exception("we could not fetch your email address from OAuth Token. Please make sure that your profile is up to date with email address."); }
            return new User { Email = userJson["email"], Name = userJson["name"], Token = App.TokenSecret, AuthRouter = "Twitter" };
        }
        private void AccessTokenGet(string authToken, string oauthVerifier)
        {
            App.Token = authToken;
            App.OAuthVerifier = oauthVerifier;
            var query = GetoAuthRequest(Method.GET, _accessTockenUrl, string.Empty);
            if (query.Length <= 0)
                return;
            var queryString = HttpUtility.ParseQueryString(query);
            if (queryString["oauth_token"] != null)
                App.Token = queryString["oauth_token"];
            if (queryString["oauth_token_secret"] != null)
                App.TokenSecret = queryString["oauth_token_secret"];
            if (queryString["screen_name"] != null)
                App.TwitterName = queryString["screen_name"];
            if (queryString["user_id"] != null)
                App.TwitterId = queryString["user_id"];
        }
        private string GetAutorizedUrl()
        {
            string str = null;
            App.Token = string.Empty; App.TokenSecret = string.Empty; App.OAuthVerifier = string.Empty;
            var query = GetoAuthRequest(Method.GET, _authorizeUrl, string.Empty);
            if (query.Length <= 0) return null;
            var queryString = HttpUtility.ParseQueryString(query);
            if (queryString["oauth_callback_confirmed"] != null && !string.Equals(queryString["oauth_callback_confirmed"], true.ToString(), StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("OAuth callback not confirmed.");
            if (queryString["oauth_token"] != null)
                str = _resourceUrl + queryString["oauth_token"];
            return str;
        }
        private string GetoAuthRequest(Method method, string url, string postData)
        {
            string normalizedUrl;
            string normalizedRequestParameters;
            if ((method == Method.POST || method == Method.DELETE || method == Method.GET) && postData.Length > 0)
            {
                var queryString = HttpUtility.ParseQueryString(postData);
                postData = string.Empty;
                foreach (var allKey in queryString.AllKeys)
                {
                    if (postData.Length > 0)
                        postData += "&";
                    queryString[allKey] = HttpUtility.UrlDecode(queryString[allKey]);
                    queryString[allKey] = UrlEncode(queryString[allKey]);
                    postData = postData + allKey + "=" + queryString[allKey];
                }
                url = url.IndexOf("?", StringComparison.Ordinal) <= 0 ? url + "?" : url + "&";
                url += postData;
            }
            var url1 = new Uri(url);
            var nonce = GenerateNonce();
            var timeStamp = GenerateTimeStamp();
            var signature = GenerateSignature(url1, _apiKey, _apiSecret, App.Token, App.TokenSecret, _callBackUrl, App.OAuthVerifier, method.ToString(), timeStamp, nonce, out normalizedUrl, out normalizedRequestParameters);
            var str = normalizedRequestParameters + "&oauth_signature=" + UrlEncode(signature);
            if (method == Method.POST || method == Method.DELETE)
            {
                postData = str;
                str = string.Empty;
            }
            if (str.Length > 0)
                normalizedUrl += "?";
            return WebRequest(method, normalizedUrl + str, postData);
        }
        private static string WebRequest(Method method, string url, string postData)
        {
            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (webRequest == null) return null;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            if (method == Method.POST || method == Method.DELETE)
            {
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
        private static string GenerateTimeStamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
        }
        private string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string callBackUrl, string oauthVerifier, string httpMethod, string timeStamp, string nonce, out string normalizedUrl, out string normalizedRequestParameters)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, callBackUrl, oauthVerifier, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters);
        }
        private string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string callBackUrl, string oauthVerifier, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;
            switch (signatureType)
            {
                case SignatureTypes.HMACSHA1:
                    var signatureBase = GenerateSignatureBase(url, consumerKey, token, callBackUrl, oauthVerifier, httpMethod, timeStamp, nonce, "HMAC-SHA1", out normalizedUrl, out normalizedRequestParameters);
                    var hmacshA1 = new HMACSHA1();
                    hmacshA1.Key = Encoding.ASCII.GetBytes($"{UrlEncode(consumerSecret)}&{(string.IsNullOrEmpty(tokenSecret) ? string.Empty : UrlEncode(tokenSecret))}");
                    return GenerateSignatureUsingHash(signatureBase, hmacshA1);
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode($"{consumerSecret}&{tokenSecret}");
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", nameof(signatureType));
            }
        }
        private string GenerateSignatureBase(Uri url, string consumerKey, string token, string callBackUrl, string oauthVerifier, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            if (token == null)
                token = string.Empty;
            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentNullException(nameof(consumerKey));
            if (string.IsNullOrEmpty(httpMethod))
                throw new ArgumentNullException(nameof(httpMethod));
            if (string.IsNullOrEmpty(signatureType))
                throw new ArgumentNullException(nameof(signatureType));
            var queryParameters = GetQueryParameters(url.Query);
            queryParameters.Add(new QueryParameter("oauth_version", "1.0"));
            queryParameters.Add(new QueryParameter("oauth_nonce", nonce));
            queryParameters.Add(new QueryParameter("oauth_timestamp", timeStamp));
            queryParameters.Add(new QueryParameter("oauth_signature_method", signatureType));
            queryParameters.Add(new QueryParameter("oauth_consumer_key", consumerKey));
            if (!string.IsNullOrEmpty(callBackUrl))
                queryParameters.Add(new QueryParameter("oauth_callback", UrlEncode(callBackUrl)));
            if (!string.IsNullOrEmpty(oauthVerifier))
                queryParameters.Add(new QueryParameter("oauth_verifier", oauthVerifier));
            if (!string.IsNullOrEmpty(token))
                queryParameters.Add(new QueryParameter("oauth_token", token));
            queryParameters.Sort(new QueryParameterComparer());
            normalizedUrl = $"{url.Scheme}://{url.Host}";
            if ((url.Scheme != "http" || url.Port != 80) && (url.Scheme != "https" || url.Port != 443))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(queryParameters);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}&", httpMethod.ToUpper());
            stringBuilder.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            stringBuilder.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));
            return stringBuilder.ToString();
        }
        private string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
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
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));
            var bytes = Encoding.ASCII.GetBytes(data);
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
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
        private class QueryParameterComparer : IComparer<QueryParameter>
        {
            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                    return string.Compare(x.Value, y.Value);
                return string.Compare(x.Name, y.Name);
            }
        }
        private enum SignatureTypes
        {
            // ReSharper disable once InconsistentNaming
            HMACSHA1,
            // ReSharper disable once InconsistentNaming
            PLAINTEXT,
            // ReSharper disable once InconsistentNaming
            RSASHA1,
        }
    }
}