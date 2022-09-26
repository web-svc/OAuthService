namespace OAuthService.Model.Twitter
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Twitter;

    public class TwitterForbidden : ITwitterForbidden
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("reason")]
        public string Error { get; set; }
        [JsonProperty("detail")]
        public string ErrorDescription { get; set; }
        [JsonProperty("registration_url")]
        public string RegistrationUrl { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("required_enrollment")]
        public string RequiredEnrollment { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
