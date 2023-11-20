using System;
using NBitcoin;
using NBitcoin.Crypto;
using System.Security.Cryptography;

class DigitalSignature
{
    public static byte[] SignData(byte[] data, byte[] privateKeyBytes)
    {
        try
        {
            // Хэширование данных с использованием SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);

                // Подписание хэша
                var key = new Key(privateKeyBytes);
                var signature = key.Sign(new uint256(hash), false);
                return signature.ToDER();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error signing data: {ex.Message}");
            return null;
        }
    }

    public static bool VerifySignature(byte[] data, byte[] signature, byte[] publicKeyBytes)
    {
        try
        {
            // Хэширование данных с использованием SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);

                // Верификация подписи
                var publicKey = new PubKey(publicKeyBytes);
                var ecdsaSignature = new ECDSASignature(signature);
                return publicKey.Verify(new uint256(hash), ecdsaSignature);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying signature: {ex.Message}");
            return false;
        }
    }

    public static string ConvertSignatureToHex(byte[] signature)
    {
        return BitConverter.ToString(signature).Replace("-", "").ToLower();
    }

    public static byte[] HexStringToByteArray(string hex)
    {
        int length = hex.Length / 2;
        byte[] bytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}