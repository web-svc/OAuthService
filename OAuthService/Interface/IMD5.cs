namespace CryptoService.Interface
{
    public interface IMD5
    {
        byte[] GetHashCode(string CipherKey);
        string Encrypt(ICipherInput cipherInput);
        string Decrypt(ICipherInput cipherInput);
    }
}
