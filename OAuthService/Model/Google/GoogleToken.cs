﻿namespace OAuthService.Model.Google
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Google;

    public class GoogleToken : IGoogleToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("id_token")]
        public string IdToken { get; set; }
        public GoogleError Error { get; set; }
    }
}