namespace OAuthService.Model.Twitter
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Twitter;

    public class TwitterOAuthError : ITwitterError
    {
        [JsonProperty("code")]
        public string Error { get; set; }
        [JsonProperty("message")]
        public string ErrorDescription { get; set; }
    }
}
