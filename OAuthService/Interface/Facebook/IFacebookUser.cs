namespace OAuthService.Interface.Facebook
{
    using OAuthService.Interface;
    using OAuthService.Model.Facebook;

    public interface IFacebookUser: IUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        FacebookError Error { get; set; }
    }
}
