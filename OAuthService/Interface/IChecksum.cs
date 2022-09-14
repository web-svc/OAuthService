namespace CryptoService.Interface
{
    public interface IChecksum
    {
        string Generate(string CipherText);
    }
}
