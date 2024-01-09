using NBitcoin;
using System;
using System.Text;

class PublicKey
{
    public static string GeneratePublicKey(string privateKeyHex)
    {
        try
        {
            byte[] privateKeyBytes = StringToByteArray(privateKeyHex);

            Key privateKey = new Key(privateKeyBytes);

            byte[] publicKeyBytes = privateKey.PubKey.ToBytes();

            return ByteArrayToString(publicKeyBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return null;
        }
    }

    private static string ByteArrayToString(byte[] array)
    {
        StringBuilder hex = new StringBuilder(array.Length * 2);
        foreach (byte b in array)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }

    public static byte[] StringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    public static byte[] GeneratePublicKeyBytes(byte[] privateKeyBytes)
    {
        Key key = new Key(privateKeyBytes);
        PubKey publicKey = key.PubKey;

        return publicKey.ToBytes();
    }
}
