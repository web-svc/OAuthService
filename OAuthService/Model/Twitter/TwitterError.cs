namespace OAuthService.Model.Twitter
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Twitter;

    public class TwitterError : ITwitterError
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
