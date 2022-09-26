namespace OAuthService.Model.Facebook
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Facebook;

    public class FacebookError : IFacebookError
    {
        [JsonProperty("type")]
        public string Error { get; set; }
        [JsonProperty("message")]
        public string ErrorDescription { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("error_subcode")]
        public string ErrorSubcode { get; set; }
        [JsonProperty("fbtrace_id")]
        public string FbTraceId { get; set; }
    }
}
