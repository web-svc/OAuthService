namespace OAuthService.Model.Microsoft
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Microsoft;
    using System.Collections.Generic;

    public class MicrosoftError : IMicrosoftError
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
        public List<string> ErrorCodes { get; set; }
        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }
        [JsonProperty("trace_id")]
        public string TraceId { get; set; }
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }
        [JsonProperty("error_uri")]
        public string ErrorUri { get; set; }
    }
}
