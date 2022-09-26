namespace OAuthService.Interface
{
    public interface IUser
    {
        string Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string AccessToken { get; set; }
        string AuthRouter { get; set; }
    }
}
