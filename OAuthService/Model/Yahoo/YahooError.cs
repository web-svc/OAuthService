namespace OAuthService.Model.Yahoo
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Yahoo;

    public class YahooError : IYahooError
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
