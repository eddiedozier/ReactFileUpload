using System;
using System.Security.Cryptography;
using System.Text;
using FileUpload.Services.Interfaces.Cryprography;
using Microsoft.AspNetCore.DataProtection;

namespace FileUpload.Services.Cryprography
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string Key = "c34f5840-f0d1-4815-9861-85154c-93bd6a-9faf6f7d-ca36-42a3-a3c8-d159c21836ca";

        public CryptographyService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Unprotect(cipherText);
        }
        
        public string GenerateRandomString(int length)
        {
            // This will give us approximately the desired length string.
            byte[] bytes = new byte[(int)Math.Floor(length * .75)];

            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return Convert.ToBase64String(bytes);
        }

        public string Hash(string message, string messHashKey)
        {

            string key = messHashKey;

            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hash;
            using (HMACSHA256 hasher = new HMACSHA256(keyBytes))
            {
                hash = hasher.ComputeHash(messageBytes);
            }
            return Convert.ToBase64String(hash);
        }

        // From https://gist.github.com/cmatskas/faee04c7b78afae065e1#file-pbkdf2dotnetsample-cs%23file-pbkdf2dotnetsample-cs .
        public string Hash(string original, string salt, int iterations = 1)
        {
            const int hashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash

            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] bytes;
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(original, saltBytes))
            {
                pbkdf2.IterationCount = iterations;
                bytes = pbkdf2.GetBytes(hashByteSize);
            }

            return Convert.ToBase64String(bytes);

        }

        // From https://gist.github.com/cmatskas/faee04c7b78afae065e1#file-pbkdf2dotnetsample-cs%23file-pbkdf2dotnetsample-cs .
        public static bool SlowEquals(string x, string y)
        {
            byte[] a = Convert.FromBase64String(x);
            byte[] b = Convert.FromBase64String(y);

            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
