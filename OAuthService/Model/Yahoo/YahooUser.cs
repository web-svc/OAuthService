namespace OAuthService.Model.Yahoo
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Yahoo;

    public class YahooUser : IYahooUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }        
        [JsonProperty("sub")]
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
        [JsonProperty("error")]
        public YahooError Error { get; set; }
        [JsonProperty("given_name")]
        public string FirstName { get; set; }
        [JsonProperty("family_name")]
        public string LastName { get; set; }
        [JsonProperty("picture")]
        public string PictureUrl { get; set; }
    }
}
