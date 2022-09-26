namespace OAuthService.Interface.Twitter
{
    public interface ITwitterForbidden : IError
    {
        string ClientId { get; set; }
        string RegistrationUrl { get; set; }
        string Title { get; set; }
        string RequiredEnrollment { get; set; }
        string Type { get; set; }
    }
}
