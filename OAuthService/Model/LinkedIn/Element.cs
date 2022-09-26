namespace OAuthService.Model.LinkedIn
{
    using Newtonsoft.Json;
    using OAuthService.Interface.LinkedIn;

    public class Element : IElement
    {
        [JsonProperty("handle~")]
        public Handle Handle { get; set; }
        public string UrnHandle { get; set; }
    }
}
