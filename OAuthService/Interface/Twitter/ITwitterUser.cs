namespace OAuthService.Interface.Twitter
{
    using OAuthService.Model.Twitter;

    public interface ITwitterUser : IUser
    {
        string ProfileImageUrl { get; set; }
        TwitterError Error { get; set; }
        TwitterForbidden Forbidden { get; set; }
    }
}
