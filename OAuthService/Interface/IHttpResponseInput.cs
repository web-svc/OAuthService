namespace OAuthService.Interface
{
    using System.Collections.Generic;

    public interface IHttpResponseInput
    {
        string BaseUri { get; set; }
        string XAuthHeader { get; set; }
        Dictionary<string, string> FormField { get; set; }
    }
}
