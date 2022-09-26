namespace OAuthService.Interface.Microsoft
{
    public interface IMicrosoftError : IError
    {
        string TimeStamp { get; set; }
        string TraceId { get; set; }
        string CorrelationId { get; set; }
        string ErrorUri { get; set; }
    }
}
