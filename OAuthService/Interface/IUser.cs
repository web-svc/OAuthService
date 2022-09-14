namespace OAuthService.Interface
{
    public interface IUser
    {
        string Name { get; set; }
        string Email { get; set; }
        string Token { get; set; }
        string AuthRouter { get; set; }
    }
}
