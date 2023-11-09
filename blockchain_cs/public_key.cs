using System;
using System.Security.Cryptography;
using System.Text;

class PublicKey
{
    public static string GeneratePublicKey(string privateKeyHex)
    {
        try
        {
            // Преобразование приватного ключа из шестнадцатеричной строки
            byte[] privateKeyBytes = StringToByteArray(privateKeyHex);

            // Создание объекта ECDSA
            using (ECDsa ecdsa = ECDsa.Create())
            {
                // Установка параметров кривой и приватного ключа
                ECParameters ecParameters = new ECParameters { Curve = ECCurve.NamedCurves.nistP256, D = privateKeyBytes };
                ecdsa.ImportParameters(ecParameters);

                // Получение публичного ключа в формате байтов
                byte[] publicKeyBytes = ecdsa.ExportSubjectPublicKeyInfo();

                // Преобразование публичного ключа в шестнадцатеричную строку
                string publicKeyHex = BitConverter.ToString(publicKeyBytes).Replace("-", "").ToLower();

                return publicKeyHex;
            }
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
}
