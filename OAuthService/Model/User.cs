namespace OAuthService.Model
{
    using OAuthService.Interface;
    public class User : IUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string AuthRouter { get; set; }
    }
}
