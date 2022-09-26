namespace OAuthService.Model.Twitter
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Twitter;
    using System.Collections.Generic;

    public class TwitterOAuthToken : ITwitterOAuthToken
    {

        [JsonProperty("oauth_token")]
        public string AccessToken { get; set; }
        [JsonProperty("oauth_token_secret")]
        public string OAuthTokenSecret { get; set; }
        [JsonProperty("oauth_callback_confirmed")]
        public bool OAuthCallbackConfirmed { get; set; }
        [JsonProperty("errors")]
        public List<TwitterOAuthError> Error { get; set; }
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
    }
}
