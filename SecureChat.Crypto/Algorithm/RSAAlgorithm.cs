using System.Security.Cryptography;
using System.Text;

namespace SecureChat.Crypto.Algorithm
{
    public static class RSAAlgorithm
    {
        private static readonly int _keySizeInBits = 3072; // 3072, which gives you 128-bit security.
        private static readonly RSAEncryptionPadding _rSAEncryptionPadding = RSAEncryptionPadding.OaepSHA3_256;
        private static readonly RSASignaturePadding _rSASignaturePadding = RSASignaturePadding.Pss;
        private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA3_384;

        public static Tuple<byte[], byte[]> GenerateKeys()
        {
            using (var rsa = RSA.Create(_keySizeInBits))
            {
                var privateRsaKey = rsa.ExportRSAPrivateKey();
                var publicRsaKey = rsa.ExportRSAPublicKey();

                return new Tuple<byte[], byte[]>(privateRsaKey, publicRsaKey);
            }
        }
        public static Task<Tuple<byte[], byte[]>> GenerateKeysAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GenerateKeys());
        }

        public static byte[] Encrypt(string plainText, byte[] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(publicKey, out var bytesRead);
                return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), _rSAEncryptionPadding);
            }
        }
        public static Task<byte[]> EncryptAsync(string plainText, byte[] publicKey, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Encrypt(plainText, publicKey));
        }

        public static string Decrypt(byte[] cipherBytes, byte[] privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(privateKey, out var bytesRead);
                var plainBytes = rsa.Decrypt(cipherBytes, _rSAEncryptionPadding);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
        public static Task<string> DecryptAsync(byte[] cipherBytes, byte[] privateKey, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Decrypt(cipherBytes, privateKey));
        }

        public static byte[] Sign(string plainText, byte[] privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(privateKey, out var bytesRead);
                return rsa.SignData(Encoding.UTF8.GetBytes(plainText), _hashAlgorithm, _rSASignaturePadding);
            }
        }
        public static Task<byte[]> SignAsync(string plainText, byte[] privateKey, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Sign(plainText, privateKey));
        }

        public static bool Verify(string plainText, byte[] signature, byte[] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(publicKey, out var bytesRead);
                return rsa.VerifyData(Encoding.UTF8.GetBytes(plainText), signature, _hashAlgorithm, _rSASignaturePadding);
            }

        }
        public static Task<bool> VerifyAsync(string plainText, byte[] signature, byte[] publicKey, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Verify(plainText, signature, publicKey));
        }
    }
}
