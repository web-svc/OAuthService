namespace OAuthService.Interface.Microsoft
{
    using OAuthService.Model.Microsoft;

    public interface IMicrosoftToken : IToken
    {
        string ExtExpiresIn { get; set; }
        string Scope { get; set; }
        MicrosoftError Error { get; set; }
    }
}
