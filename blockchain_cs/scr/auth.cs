﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

public class WalletData
{
    public string Address { get; set; }
    public string Phrase { get; set; }
    public string PublicKey { get; set; }
}

public class BinaryFileHandler
{
    private string walletsFolderPath = "wallets";

    public BinaryFileHandler()
    {
        if (!Directory.Exists(walletsFolderPath))
        {
            Directory.CreateDirectory(walletsFolderPath);
        }
    }

    public void RegisterWallet(WalletData walletData)
    {
        string walletFilePath = Path.Combine(walletsFolderPath, $"{walletData.Address}.dat");

        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(walletFilePath, FileMode.Create)))
            {
                writer.Write(walletData.Address);

                string Phrase = walletData.Phrase;
                string publicKeyHex = walletData.PublicKey;

                writer.Write(Phrase);
                writer.Write(publicKeyHex);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to binary file: {ex.Message}");
        }
    }

    public Dictionary<string, string> ReadWalletData(string address)
    {
        string walletFilePath = Path.Combine(walletsFolderPath, $"{address}.dat");

        if (File.Exists(walletFilePath))
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(walletFilePath)))
                {
                    string savedAddress = reader.ReadString();
                    string phrase = reader.ReadString();
                    string publicKey = reader.ReadString();
                    string signatureKey = reader.ReadString();

                    Dictionary<string, string> walletDataDictionary = new Dictionary<string, string>
                {
                    { "Address", savedAddress },
                    { "Phrase",  phrase },
                    { "PublicKey", publicKey },
                    { "Signature", signatureKey }
                };

                    return walletDataDictionary;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading wallet data: {ex.Message}");
            }
        }

        return null;
    }

    public void ClearWalletFile(string address)
    {
        string walletFilePath = Path.Combine(walletsFolderPath, $"{address}.dat");

        try
        {
            if (File.Exists(walletFilePath))
            {
                File.Delete(walletFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting wallet file: {ex.Message}");
        }
    }

    private string ComputeSHA256Hash(string rawData)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}

