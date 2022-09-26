namespace OAuthService.Interface.Google
{
    using OAuthService.Model.Google;

    public interface IGoogleToken : IToken
    {
        string RefreshToken { get; set; }
        string Scope { get; set; }
        string IdToken { get; set; }
        GoogleError Error { get; set; }
    }
}
