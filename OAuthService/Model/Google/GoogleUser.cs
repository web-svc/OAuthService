namespace OAuthService.Model.Google
{
    using Newtonsoft.Json;
    using OAuthService.Interface.Google;

    public class GoogleUser : IGoogleUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }        
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonProperty("given_name")]
        public string FirstName { get; set; }
        [JsonProperty("family_name")]
        public string LastName { get; set; }
        [JsonProperty("picture")]
        public string PictureUri { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
        [JsonProperty("error")]
        public GoogleUserError Error { get; set; }
    }
}
