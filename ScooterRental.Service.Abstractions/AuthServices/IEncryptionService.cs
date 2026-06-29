namespace ScooterRental.Service.Abstractions.AuthServices
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
