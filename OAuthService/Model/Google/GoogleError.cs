namespace OAuthService.Model.Google
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Google;

    public class GoogleError : IGoogleError
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
