namespace OAuthService.Model.Microsoft
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Microsoft;

    public class MicrosoftUser : IMicrosoftUser
    {
        [JsonProperty("displayName")]
        public string Name { get; set; }
        [JsonProperty("userPrincipalName")]
        public string Email { get; set; }        
        [JsonProperty("id")]
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
        [JsonProperty("error")]
        public MicrosoftError Error { get; set; }
        [JsonProperty("givenName")]
        public string FirstName { get; set; }
        [JsonProperty("surname")]
        public string LastName { get; set; }
    }
}
