namespace OAuthService.Model
{
    using Newtonsoft.Json;
    using OAuthService.Interface;
    public class Token : IToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
    }
}
