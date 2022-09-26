namespace OAuthService.Model.LinkedIn
{
    using Newtonsoft.Json;
    using OAuthService.Interface.LinkedIn;

    public class LinkedInError : ILinkedInError
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
