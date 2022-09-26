namespace OAuthService.Interface.Yahoo
{
    using OAuthService.Model.Yahoo;

    public interface IYahooUser : IUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string PictureUrl { get; set; }
        YahooError Error { get; set; }
    }
}
