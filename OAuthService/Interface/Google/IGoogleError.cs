namespace OAuthService.Interface.Google
{
    public interface IGoogleError: IError
    {
        string Code { get; set; }
    }
}
