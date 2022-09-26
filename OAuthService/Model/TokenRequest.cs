namespace OAuthService.Model
{
    using OAuthService.Interface;
    using System.Collections.Specialized;

    class TokenRequest : ITokenRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Code { get; set; }
        public string GrantType { get; set; }
        public string BaseUri { get; set; }
        public string BasicXAuthCode { get; set; }
        public string CsrfState { get; set; }
        public string TimeStamp { get; set; }
        public string SignatureMethod { get; set; }
        public NameValueCollection QueryString { get; set; }
    }
}
