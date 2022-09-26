namespace OAuthService.Interface.Twitter
{
    using OAuthService.Model.Twitter;
    using System.Collections.Generic;

    public interface ITwitterOAuthToken : IToken
    {
        string OAuthTokenSecret { get; set; }
        bool OAuthCallbackConfirmed { get; set; }
        List<TwitterOAuthError> Error { get; set; }
    }
}
