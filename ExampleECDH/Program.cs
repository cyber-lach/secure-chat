﻿using System.Security.Cryptography;
using System.Text;

class Alice
{
    public static byte[] alicePublicKey;

    public static void Main(string[] args)
    {
        using (ECDiffieHellmanCng alice = new ECDiffieHellmanCng())
        {

            alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            alice.HashAlgorithm = CngAlgorithm.Sha256;
            alicePublicKey = alice.PublicKey.ToByteArray();
            Bob bob = new Bob();
            CngKey bobKey = CngKey.Import(bob.bobPublicKey, CngKeyBlobFormat.EccPublicBlob);
            byte[] aliceKey = alice.DeriveKeyMaterial(bobKey);
            byte[] encryptedMessage = null;
            byte[] iv = null;
            Send(aliceKey, "Secret message", out encryptedMessage, out iv);
            Console.WriteLine($"Plain text: Secret message");
            Console.WriteLine($"Alice key: {Convert.ToBase64String(alicePublicKey)}");
            Console.WriteLine($"Alice's shared secret: {Convert.ToBase64String(aliceKey)}");
            Console.WriteLine($"Bob key: {Convert.ToBase64String(bob.bobPublicKey)}");
            Console.WriteLine($"Bob's shared secret: {Convert.ToBase64String(bob.bobKey)}");
            Console.WriteLine($"Encrypted message: {Convert.ToBase64String(encryptedMessage)}");
            Console.WriteLine(Convert.ToBase64String(encryptedMessage));
            bob.Receive(encryptedMessage, iv);
        }
    }

    private static void Send(byte[] key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
    {
        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            iv = aes.IV;

            // Encrypt the message
            using (MemoryStream ciphertext = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                cs.Close();
                encryptedMessage = ciphertext.ToArray();
            }
        }
    }
}
public class Bob
{
    public byte[] bobPublicKey;
    public byte[] bobKey;
    public Bob()
    {
        using (ECDiffieHellmanCng bob = new ECDiffieHellmanCng())
        {

            bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            bob.HashAlgorithm = CngAlgorithm.Sha256;
            bobPublicKey = bob.PublicKey.ToByteArray();
            bobKey = bob.DeriveKeyMaterial(CngKey.Import(Alice.alicePublicKey, CngKeyBlobFormat.EccPublicBlob));
        }
    }

    public void Receive(byte[] encryptedMessage, byte[] iv)
    {

        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = bobKey;
            aes.IV = iv;
            // Decrypt the message
            using (MemoryStream plaintext = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                    cs.Close();
                    string message = Encoding.UTF8.GetString(plaintext.ToArray());
                    Console.WriteLine($"Decrypted message: {message}");
                }
            }
        }
    }
}