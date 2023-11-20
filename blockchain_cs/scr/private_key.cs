using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using NBitcoin;

class PrivateKey
{
    public static string generate_private_key(string phrase)
    {
        byte[] mnemonicBytes = Encoding.UTF8.GetBytes(phrase);
        string private_key = Sha256.ComputeSHA256Hash(phrase);
        bool validate_private_key = false;

        if (ValidatePrivateKey(private_key))
        {   
            validate_private_key = true;
            return private_key;
        }
        else
        {
            while (validate_private_key == false)
            {
                return generate_private_key(phrase);
            }
            return private_key;
        }

    }

    public static bool ValidatePrivateKey(string private_key)
    {
        string maxPrivateKeyHex = "FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFE BAAEDCE6 AF48A03B BFD25E8C D0364140";
        return String.Compare(private_key, maxPrivateKeyHex, StringComparison.OrdinalIgnoreCase) <= 0;
    }

    public static byte[] GeneratePrivateKeyBytes(string mnemonic)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] seed = Encoding.UTF8.GetBytes(mnemonic);
            return sha256.ComputeHash(seed);
        }
    }

    public static byte[] GetPrivateKeyBytes(string privateKeyHash)
    {
        try
        {
            // Преобразование строки хэша в массив байтов
            byte[] hashBytes = StringToByteArray(privateKeyHash);

            // Добавьте дополнительные операции по вашему усмотрению,
            // чтобы получить байты приватного ключа из хэша.
            // Ниже просто преобразование хэша обратно в байты.

            return hashBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting private key bytes: {ex.Message}");
            return null;
        }
    }

    static byte[] StringToByteArray(string hex)
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