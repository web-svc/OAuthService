namespace OAuthService.Model.Microsoft
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Microsoft;

    public class MicrosoftToken : IMicrosoftToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("ext_expires_in")]
        public string ExtExpiresIn { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        public MicrosoftError Error { get; set; }
    }
}
