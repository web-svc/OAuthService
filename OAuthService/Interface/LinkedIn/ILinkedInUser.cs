namespace OAuthService.Interface.LinkedIn
{
    using OAuthService.Model.LinkedIn;
    using System.Collections.Generic;

    public interface ILinkedInUser : IUser
    {
        List<Element> Elements { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        LinkedInError Error { get; set; }
    }
}
