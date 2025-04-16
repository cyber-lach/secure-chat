using System.Security.Cryptography;

namespace ExampleRSA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("***** Asymmetric encryption demo *****");

            var unencryptedMessage = "To be or not to be, that is the question, whether tis nobler in the...";
            Console.WriteLine("Unencrypted message: " + unencryptedMessage);

            // 1. Create a public / private key pair.
            RSAParameters privateAndPublicKeys, publicKeyOnly;
            byte[] privateBytes, publicBytes;
            string privateXml, publicXml;
            using (var rsaAlg = RSA.Create(2048))
            {
                //privateAndPublicKeys = rsaAlg.ExportParameters(includePrivateParameters: true);
                //publicKeyOnly = rsaAlg.ExportParameters(includePrivateParameters: false);
                //privateXml = rsaAlg.ToXmlString(true);
                //publicXml = rsaAlg.ToXmlString(false);
                privateBytes = rsaAlg.ExportRSAPrivateKey();
                publicBytes = rsaAlg.ExportRSAPublicKey();
            }

            // 2. Sender: Encrypt message using public key
            //var encryptedMessage = AsymmetricEncryption.Encrypt(unencryptedMessage, privateAndPublicKeys);
            //var encryptedMessage = AsymmetricEncryption.Encrypt(unencryptedMessage, publicXml);
            var encryptedMessage = AsymmetricEncryption.Encrypt(unencryptedMessage, publicBytes);
            Console.WriteLine("Sending encrypted message: " + Convert.ToBase64String(encryptedMessage));

            // 3. Receiver: Decrypt message using private key
            //var decryptedMessage = AsymmetricEncryption.Decrypt(encryptedMessage, publicKeyOnly);
            //var decryptedMessage = AsymmetricEncryption.Decrypt(encryptedMessage, privateXml);
            var decryptedMessage = AsymmetricEncryption.Decrypt(encryptedMessage, privateBytes);
            Console.WriteLine("Recieved and decrypted message: " + decryptedMessage);


            var signature = AsymmetricEncryption.Sign(unencryptedMessage, privateBytes);
            var verified = AsymmetricEncryption.Verify(unencryptedMessage, signature, publicBytes);

            Console.WriteLine("Verified signature: " + verified);

            signature = AsymmetricEncryption.Sign(unencryptedMessage, privateBytes);
            verified = AsymmetricEncryption.Verify(unencryptedMessage, signature, publicBytes);
            verified = AsymmetricEncryption.Verify(unencryptedMessage.ToLower(), signature, publicBytes);

            Console.WriteLine("Verified signature: " + verified);

            Console.Write(Environment.NewLine);
        }
    }
}
