// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

class AliceOpenSsl
{
    public static ECDiffieHellmanPublicKey alicePublicKey;

    public static void Main2(string[] args)
    {
        using (var alice = new ECDiffieHellmanOpenSsl()) //Initializes a new instance of the ECDiffieHellmanOpenSsl class with a default curve of NIST P-521/secp521r1 https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.ecdiffiehellmanopenssl.-ctor?view=net-9.0
        {

            alicePublicKey = alice.PublicKey;
            BobOpenSsl bob = new BobOpenSsl();
            byte[] alicePrivateKey = alice.DeriveKeyMaterial(bob.bobPublicKey);
            byte[] encryptedMessage = null;
            byte[] iv = null;
            Send(alicePrivateKey, "Secret message", out encryptedMessage, out iv);
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

internal class BobOpenSsl
{
    public ECDiffieHellmanPublicKey bobPublicKey;
    private byte[] bobPrivateKey;
    public BobOpenSsl()
    {
        using (var bob = new ECDiffieHellmanOpenSsl())
        {

            bobPublicKey = bob.PublicKey;
            bobPrivateKey = bob.DeriveKeyMaterial(AliceOpenSsl.alicePublicKey);
        }
    }

    public void Receive(byte[] encryptedMessage, byte[] iv)
    {

        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = bobPrivateKey;
            aes.IV = iv;
            // Decrypt the message
            using (MemoryStream plaintext = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                    cs.Close();
                    string message = Encoding.UTF8.GetString(plaintext.ToArray());
                    Console.WriteLine(message);
                }
            }
        }
    }
}