namespace OAuthService.Model
{
    using Newtonsoft.Json;
    using OAuthService.Interface;
    public class User : IUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }        
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
    }
}
