using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

class AddressGenerator
{
    public static string GenerateReadableAddress(string hash)
    {
        try
        {
            byte[] hashBytes = StringToByteArray(hash);

            byte[] versionedBytes = new byte[hashBytes.Length + 1];
            versionedBytes[0] = 0x00;
            Array.Copy(hashBytes, 0, versionedBytes, 1, hashBytes.Length);

            byte[] checksum = CalculateChecksum(versionedBytes);

            byte[] totalBytes = new byte[versionedBytes.Length + checksum.Length];
            Array.Copy(versionedBytes, 0, totalBytes, 0, versionedBytes.Length);
            Array.Copy(checksum, 0, totalBytes, versionedBytes.Length, checksum.Length);

            string base58CheckAddress = Base58CheckEncode(totalBytes);

            return base58CheckAddress;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return null;
        }
    }

    private static byte[] StringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    private static byte[] CalculateChecksum(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash1 = sha256.ComputeHash(data);
            byte[] hash2 = sha256.ComputeHash(hash1);
            byte[] checksum = new byte[4];
            Array.Copy(hash2, checksum, 4);
            return checksum;
        }
    }

    private static string Base58CheckEncode(byte[] data)
    {
        const string base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        StringBuilder result = new StringBuilder();

        BigInteger value = new BigInteger(data.Reverse().ToArray());

        while (value > 0)
        {
            result.Insert(0, base58Chars[(int)(value % 58)]);
            value /= 58;
        }

        for (int i = 0; i < data.Length && data[i] == 0; i++)
        {
            result.Insert(0, '1');
        }

        return result.ToString();
    }
}
