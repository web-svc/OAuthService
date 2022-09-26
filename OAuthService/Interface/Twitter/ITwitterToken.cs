namespace OAuthService.Interface.Twitter
{
    using OAuthService.Model.Twitter;

    public interface ITwitterToken : IToken
    {
        string Scope { get; set; }
        string RefreshToken { get; set; }
        TwitterError Error { get; set; }
    }
}
