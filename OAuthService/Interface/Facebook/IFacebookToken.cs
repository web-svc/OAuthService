namespace OAuthService.Interface.Facebook
{
    using OAuthService.Model.Facebook;

    public interface IFacebookToken: IToken
    {
        FacebookError Error { get; set; }
    }
}
