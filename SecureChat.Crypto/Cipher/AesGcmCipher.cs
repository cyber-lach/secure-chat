using System.Security.Cryptography;
using System.Text;

namespace SecureChat.Crypto.Cipher
{
    public static class AesGcmCipher
    {
    public static Task<byte[]> EncryptAsync(string plainText, byte[] key)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        using (var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize))
        {
            // AesGcm.NonceByteSizes.MaxSize = 12 bytes
            // AesGcm.TagByteSizes.MaxSize = 16 bytes
            Span<byte> buffer = new byte[plainBytes.Length 
                + AesGcm.NonceByteSizes.MaxSize 
                + AesGcm.TagByteSizes.MaxSize];
            var nonce = buffer.Slice(plainBytes.Length, 
                AesGcm.NonceByteSizes.MaxSize);
            RandomNumberGenerator.Fill(nonce);
            aes.Encrypt(nonce, 
                plainBytes, 
                buffer.Slice(0, plainBytes.Length), 
                buffer.Slice(plainBytes.Length + AesGcm.NonceByteSizes.MaxSize, 
                AesGcm.TagByteSizes.MaxSize));
            // buffer has encrypted data bytes + 12 bytes of Nonce + 16 bytes of Tag
            return Task.FromResult(buffer.ToArray());
        }
    }

    public static Task<string> DecryptAsync(byte[] encryptedBytes, byte[] key)
    {
        Span<byte> encryptedData = encryptedBytes;
            
        // encryptedData has encrypted data bytes + 12 bytes of Nonce + 16 bytes of Tag
        var tag = encryptedData.Slice(encryptedData.Length - AesGcm.TagByteSizes.MaxSize, 
            AesGcm.TagByteSizes.MaxSize);
        var nonce = encryptedData.Slice(encryptedData.Length 
            - AesGcm.TagByteSizes.MaxSize 
            - AesGcm.NonceByteSizes.MaxSize, AesGcm.NonceByteSizes.MaxSize);
        var cipherBytes = encryptedData.Slice(0, encryptedData.Length 
            - AesGcm.TagByteSizes.MaxSize
            - AesGcm.NonceByteSizes.MaxSize);
        Span<byte> buffer = new byte[cipherBytes.Length];
        using (var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize))
        {
            aes.Decrypt(nonce, cipherBytes, tag, buffer);
        }

        return Task.FromResult(Encoding.UTF8.GetString(buffer));
    }
    }
}
