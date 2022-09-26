namespace OAuthService.Interface
{
    public interface IToken
    {
        string AccessToken { get; set; }

        string TokenType { get; set; }

        string ExpiresIn { get; set; }
    }
}
