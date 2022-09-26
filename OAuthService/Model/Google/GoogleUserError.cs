namespace OAuthService.Model.Google
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Google;

    public class GoogleUserError : IGoogleError
    {
        [JsonProperty("status")]
        public string Error { get; set; }
        [JsonProperty("message")]
        public string ErrorDescription { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
