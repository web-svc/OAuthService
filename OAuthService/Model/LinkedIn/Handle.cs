namespace OAuthService.Model.LinkedIn
{
    using Newtonsoft.Json;
    using OAuthService.Interface.LinkedIn;

    public class Handle : IHandle
    {
        [JsonProperty("emailAddress")]
        public string Email { get; set; }
    }
}
