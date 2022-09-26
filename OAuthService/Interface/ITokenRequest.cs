namespace OAuthService.Interface
{
    using System.Collections.Specialized;

    public interface ITokenRequest
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string RedirectUri { get; set; }
        string Code { get; set; }
        string GrantType { get; set; }
        string BaseUri { get; set; }
        string BasicXAuthCode { get; set; }
        string CsrfState { get; set; }
        NameValueCollection QueryString { get; set; }
    }
}
