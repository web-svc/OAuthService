namespace OAuthService.Interface.Yahoo
{
    using OAuthService.Model.Yahoo;

    public interface IYahooToken : IToken
    {
        string RefreshToken { get; set; }
        YahooError Error { get; set; }
    }
}
