using System.Security.Cryptography;
using System.Text;

namespace ExampleRSA
{
    internal class AsymmetricEncryption
    {
        internal static byte[] Encrypt(string message, RSAParameters rsaParameters)
        {
            using (var rsaAlg = RSA.Create(rsaParameters))
            {
                return rsaAlg.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA3_256);
            }
        }

        internal static string Decrypt(byte[] cipherText, RSAParameters rsaParameters)
        {
            using (var rsaAlg = RSA.Create(rsaParameters))
            {
                var decryptedMessage = rsaAlg.Decrypt(cipherText, RSAEncryptionPadding.OaepSHA3_256);
                return Encoding.UTF8.GetString(decryptedMessage);
            }

        }

        internal static byte[] Encrypt(string message, byte[] rsaKey)
        {
            using (var rsaAlg = RSA.Create())
            {
                rsaAlg.ImportRSAPublicKey(rsaKey, out var bytesRead);
                return rsaAlg.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA3_256);
            }
        }

        internal static string Decrypt(byte[] cipherText, byte[] rsaKey)
        {
            using (var rsaAlg = RSA.Create())
            {
                rsaAlg.ImportRSAPrivateKey(rsaKey, out var bytesRead);
                var decryptedMessage = rsaAlg.Decrypt(cipherText, RSAEncryptionPadding.OaepSHA3_256);
                return Encoding.UTF8.GetString(decryptedMessage);
            }

        }

        internal static byte[] Encrypt(string message, string rsaKey)
        {
            using (var rsaAlg = RSA.Create())
            {
                rsaAlg.FromXmlString(rsaKey);
                return rsaAlg.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA3_256);
            }
        }

        internal static string Decrypt(byte[] cipherText, string rsaKey)
        {
            using (var rsaAlg = RSA.Create())
            {
                rsaAlg.FromXmlString(rsaKey);
                var decryptedMessage = rsaAlg.Decrypt(cipherText, RSAEncryptionPadding.OaepSHA3_256);
                return Encoding.UTF8.GetString(decryptedMessage);
            }

        }


        internal static byte[] Sign(string message, byte[] rsaKey)
        {
            using (var rsaAlg = RSA.Create())
            {
                rsaAlg.ImportRSAPrivateKey(rsaKey, out var bytesRead);
                return rsaAlg.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA3_384, RSASignaturePadding.Pss);
            }
        }

        internal static bool Verify(string plaintext, byte[] signature, byte[] rsaKey)
        {
            using (var rsaAlg = RSA.Create())
            {
                rsaAlg.ImportRSAPublicKey(rsaKey, out var bytesRead);
                return rsaAlg.VerifyData(Encoding.UTF8.GetBytes(plaintext), signature, HashAlgorithmName.SHA3_384, RSASignaturePadding.Pss);
            }

        }
    }
}
