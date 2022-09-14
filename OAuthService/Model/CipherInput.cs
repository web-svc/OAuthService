namespace CryptoService.Model
{
    using CryptoService.Interface;
    using System.ComponentModel.DataAnnotations;

    public class CipherInput : ICipherInput
    {
        [Required]
        public string CipherText { get; set; }
        [Required]
        public string CipherKey { get; set; }
        [Required]
        public bool UseHashing { get; set; } = false;
    }
}
