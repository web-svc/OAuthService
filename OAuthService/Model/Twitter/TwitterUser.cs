namespace OAuthService.Model.Twitter
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Twitter;

    public class TwitterUser : ITwitterUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }        
        [JsonProperty("id")]
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
        [JsonProperty("error")]
        public TwitterError Error { get; set; }
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
        public TwitterForbidden Forbidden { get; set; }
    }
}
