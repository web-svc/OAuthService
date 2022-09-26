namespace OAuthService.Model.Facebook
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Facebook;

    public class FacebookUser : IFacebookUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }        
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
        public FacebookError Error { get; set; }
    }
}
