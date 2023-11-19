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
            // Преобразование шестнадцатеричной строки в массив байтов
            byte[] hashBytes = StringToByteArray(hash);

            // Добавление версии (0x00 для основного адреса Bitcoin)
            byte[] versionedBytes = new byte[hashBytes.Length + 1];
            versionedBytes[0] = 0x00;
            Array.Copy(hashBytes, 0, versionedBytes, 1, hashBytes.Length);

            // Расчет контрольной суммы
            byte[] checksum = CalculateChecksum(versionedBytes);

            // Создание массива для хранения полной версии и контрольной суммы
            byte[] totalBytes = new byte[versionedBytes.Length + checksum.Length];
            Array.Copy(versionedBytes, 0, totalBytes, 0, versionedBytes.Length);
            Array.Copy(checksum, 0, totalBytes, versionedBytes.Length, checksum.Length);

            // Применение Base58Check кодирования для создания адреса
            string base58CheckAddress = Base58CheckEncode(totalBytes);

            // Возвращение адреса
            return base58CheckAddress;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return null;
        }
    }

    // Вспомогательная функция для преобразования шестнадцатеричной строки в массив байтов
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

    // Вспомогательная функция для расчета контрольной суммы
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

    // Вспомогательная функция для Base58Check кодирования
    private static string Base58CheckEncode(byte[] data)
    {
        const string base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        StringBuilder result = new StringBuilder();

        // Преобразование байтов в число BigInt
        BigInteger value = new BigInteger(data.Reverse().ToArray());

        // Кодирование в системе Base58
        while (value > 0)
        {
            result.Insert(0, base58Chars[(int)(value % 58)]);
            value /= 58;
        }

        // Добавление символов '1' в начало для представления нулевых байтов
        for (int i = 0; i < data.Length && data[i] == 0; i++)
        {
            result.Insert(0, '1');
        }

        return result.ToString();
    }
}
