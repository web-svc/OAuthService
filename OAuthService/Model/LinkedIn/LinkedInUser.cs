namespace OAuthService.Model.LinkedIn
{
    using Newtonsoft.Json;
    using OAuthService.Interface.LinkedIn;
    using System.Collections.Generic;

    public class LinkedInUser : ILinkedInUser
    {
        public string AccessToken { get; set; }
        public string AuthRouter { get; set; }
        [JsonProperty("error")]
        public LinkedInError Error { get; set; }
        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonProperty("localizedFirstName")]
        public string FirstName { get; set; }
        [JsonProperty("localizedLastName")]
        public string LastName { get; set; }
    }
}
