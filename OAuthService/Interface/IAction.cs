namespace CryptoService.Interface
{
    public interface IAction
    {
        string Encrypt(ICipherInput cipherInput);
        string Decrypt(ICipherInput cipherInput);
    }
}
