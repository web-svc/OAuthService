namespace OAuthService.Model.LinkedIn
{
    using Newtonsoft.Json;
    using OAuthService.Interface.LinkedIn;

    public class LinkedInToken : ILinkedInToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        public LinkedInError Error { get; set; }
    }
}
