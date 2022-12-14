namespace OAuthService.Model.Yahoo
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Yahoo;

    public class YahooToken : IYahooToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public YahooError Error { get; set; }
    }
}
