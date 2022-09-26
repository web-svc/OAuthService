namespace OAuthService.Model.Facebook
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Facebook;

    public class FacebookToken : IFacebookToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("error")]
        public FacebookError Error { get; set; }
    }
}
