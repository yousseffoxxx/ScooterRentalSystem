namespace ScooterRental.Service.AuthServices
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(IConfiguration configuration)
        {
            // IMPORTANT: Add a 32-character "EncryptionKey" and 16-character "EncryptionIV" to your appsettings.json!
            _key = Encoding.UTF8.GetBytes(configuration["SecuritySettings:EncryptionKey"]);
            _iv = Encoding.UTF8.GetBytes(configuration["SecuritySettings:EncryptionIV"]);
        }

        public string Encrypt(string plainText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = _key;
            aesAlg.IV = _iv;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = _key;
            aesAlg.IV = _iv;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
