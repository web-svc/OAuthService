namespace OAuthService.Interface.Microsoft
{
    using OAuthService.Model.Microsoft;

    public interface IMicrosoftUser : IUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        MicrosoftError Error { get; set; }
    }
}
