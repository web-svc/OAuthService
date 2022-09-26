namespace OAuthService.Interface.LinkedIn
{
    using OAuthService.Model.LinkedIn;

    public interface ILinkedInToken : IToken
    {
        LinkedInError Error { get; set; }
    }
}
