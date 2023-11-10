using Org.BouncyCastle.Crypto;
using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        string mnemonic_phrase = MnemonicGenerator.generate_phrase();
        string privateKeyHexExample = PrivateKey.generate_private_key(mnemonic_phrase);
        string publicKeyExample = PublicKey.GeneratePublicKey(privateKeyHexExample);
        string address = AddressGenerator.GenerateReadableAddress(publicKeyExample);
        byte[] privateKeyBytes = PrivateKey.GeneratePrivateKeyBytes(mnemonic_phrase);
        byte[] publicKeyBytes = PublicKey.GeneratePublicKeyBytes(privateKeyBytes);

        byte[] dataToSign = Encoding.UTF8.GetBytes("Hello, World!");

        // Хэширование данных
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedData = sha256.ComputeHash(dataToSign);

            // Подпись хэшированных данных
            byte[] signature = DigitalSignature.SignData(hashedData, privateKeyBytes);
            bool isSignatureValid = DigitalSignature.VerifySignature(hashedData, signature, publicKeyBytes);
            Console.WriteLine($"Is signature valid: {isSignatureValid}");
        }

    }
}