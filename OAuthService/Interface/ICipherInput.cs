namespace CryptoService.Interface
{
    public interface ICipherInput
    {
        string CipherText { get; set; }
        string CipherKey { get; set; }
        bool UseHashing { get; set; }
    }
}
