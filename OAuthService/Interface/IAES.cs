namespace CryptoService.Interface
{
    public interface IAES
    {
        string Encrypt(ICipherInput cipherInput);
        string Decrypt(ICipherInput cipherInput);
    }
}
