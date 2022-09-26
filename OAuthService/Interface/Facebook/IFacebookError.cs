namespace OAuthService.Interface.Facebook
{
    public interface IFacebookError : IError
    {
        string Code { get; set; }
        string ErrorSubcode { get; set; }
        string FbTraceId { get; set; }
    }
}
