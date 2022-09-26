namespace OAuthService.Interface
{
    public interface IError
    {
        string Error { get; set; }
        string ErrorDescription { get; set; }
    }
}
