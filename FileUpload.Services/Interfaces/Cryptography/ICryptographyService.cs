namespace FileUpload.Services.Interfaces.Cryprography
{
    public interface ICryptographyService
    {
        string Decrypt(string cipherText);
        string Encrypt(string input);
        string GenerateRandomString(int length);
        string Hash(string message, string messHashKey);
        string Hash(string original, string salt, int iterations = 1);
    }
}