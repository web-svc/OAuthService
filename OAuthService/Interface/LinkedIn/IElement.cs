namespace OAuthService.Interface.LinkedIn
{
    using OAuthService.Model.LinkedIn;

    internal interface IElement
    {
        Handle Handle { get; set; }
        string UrnHandle { get; set; }
    }
}
