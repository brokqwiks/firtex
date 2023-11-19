using System.Collections.Generic;
using System.Numerics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Security.Cryptography;
using Org.BouncyCastle;
using System;
using System.IO;
using System.Linq;
using NBitcoin;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;




public class WalletData
{
    public string Address { get; set; }
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public string SignatureKey { get; set; }
}

public class BinaryFileHandler
{
    private string filePath = "allUserData.dat";
    public void RegisterWallet(WalletData WalletData)
    {
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.Write(WalletData.Address);

                string PrivateKeyHex = Sha256.ComputeSHA256Hash(WalletData.PrivateKey);
                string PublicKeyHex = Sha256.ComputeSHA256Hash(WalletData.PublicKey);
                writer.Write(PrivateKeyHex);
                writer.Write(PublicKeyHex);
                writer.Write(WalletData.SignatureKey);
                writer.Write("\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при записи в бинарный файл: {ex.Message}");
        }
    }

    public Dictionary<string, string> ReadWalletData()
    {
        Dictionary<string, string> WalletDataDictionary = new Dictionary<string, string>();

        if (File.Exists(filePath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    // Пропускаем разделитель, если это не начало файла
                    if (reader.BaseStream.Position != 0)
                    {
                        reader.ReadString(); // Пропускаем "\n"
                    }

                    // Читаем данные пользователя
                    string Address = reader.ReadString();
                    string PrivateKey = reader.ReadString();
                    string PublicKey = reader.ReadString();
                    string SignatureKey = reader.ReadString();

                    // Добавляем данные в словарь
                    WalletDataDictionary.Add("Address", Address);
                    WalletDataDictionary.Add("PrivateKey", PrivateKey);
                    WalletDataDictionary.Add("PublicKey", PublicKey);
                    WalletDataDictionary.Add("Signature", SignatureKey);
                    return WalletDataDictionary;
                }
            }
        }
        return null;
    }

    public void ClearFile()
    {
        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении бинарного файла: {ex.Message}");
        }
    }



}
