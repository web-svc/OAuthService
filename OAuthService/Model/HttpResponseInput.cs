namespace OAuthService.Model
{
    using OAuthService.Interface;
    using System.Collections.Generic;

    public class HttpResponseInput : IHttpResponseInput
    {
        public string BaseUri { get; set; }
        public string XAuthHeader { get; set; }
        public Dictionary<string, string> FormField { get; set; }
    }
}
