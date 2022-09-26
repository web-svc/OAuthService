namespace OAuthService.Interface.Google
{
    using OAuthService.Model.Google;

    public interface IGoogleUser : IUser
    {
        bool VerifiedEmail { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PictureUri { get; set; }
        string Locale { get; set; }
        GoogleUserError Error { get; set; }
    }
}
