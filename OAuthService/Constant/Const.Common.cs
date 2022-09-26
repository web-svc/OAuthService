namespace OAuthService.Constant
{
    internal partial class Const
    {
        internal const string GrantType = "grant_type";
        internal const string GrantTypeValue = "authorization_code";
        internal const string ResponseType = "response_type";
        internal const string ResponseTypeValue = "code";
        internal const string CsrfState = "state";
        internal const string AuthCode = "code";
        internal const string Scope = "scope";
        internal const string ClientId = "client_id";
        internal const string RedirectUri = "redirect_uri";
        internal const string ClientSecret = "client_secret";
        internal const string Error = "error";
        internal const string AuthHeaderKey = "Authorization";
        internal const string InvalidCsrf = "Invalid Csrf state received from \"Open Auth Provider\". Either it has been tempered or changed.";
        internal const string InvalidToken = "Invalid \"Open Auth Provider\" token.";
    }
}