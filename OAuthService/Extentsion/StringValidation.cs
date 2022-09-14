namespace OAuthService.Extentsion
{
    internal static class StringValidation
    {
        internal static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
