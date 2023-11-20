using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

public class Block
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; }
    public string PreviousBlockHash { get; set; }
    public string BlockHash { get; set; }
    public string PublicKey { get; set; }
    public string Signature_Key { get; set; }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Timestamp.ToBinary());
        writer.Write(Data);
        writer.Write(PreviousBlockHash);
        writer.Write(BlockHash);
        writer.Write(Signature_Key);
    }

    public void Deserialize(BinaryReader reader)
    {
        Index = reader.ReadInt32();
        Timestamp = DateTime.FromBinary(reader.ReadInt64());
        Data = reader.ReadString();
        PreviousBlockHash = reader.ReadString();
        BlockHash = reader.ReadString();
        Signature_Key = reader.ReadString();
    }

    public string GetBlockFileName()
    {
        return $"{Index}_block.dat";
    }
}

public class Blockchain
{
    public List<Block> blocks = new List<Block>();
    private string folderPath = "blockchain_blocks";

    public void AddBlock(Block block)
    {
        blocks.Add(block);
        SaveToFile(block);
    }

    public void LoadFromFile()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        FileInfo[] files = directoryInfo.GetFiles("*.dat");

        foreach (FileInfo file in files)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(file.FullName)))
            {
                Block block = new Block();
                block.Deserialize(reader);
                blocks.Add(block);
            }
        }

        if (blocks.Count == 0)
        {
            // Если файлы не существуют, создаем генезис-блок
            CreateGenesisBlock();
        }
    }

    private void SaveToFile(Block block)
    {
        string filePath = Path.Combine(folderPath, block.GetBlockFileName());
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            block.Serialize(writer);
        }
    }

    public void ClearFiles()
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
    }

    public Block GetLastBlock()
    {
        if (blocks.Count > 0)
        {
            return blocks[blocks.Count - 1];
        }
        return null;
    }

    private void CreateGenesisBlock()
    {
        // Создаем генезис-блок
        Block genesisBlock = new Block
        {
            Index = 0,
            Timestamp = DateTime.Now,
            Data = "Genesis Block",
            PreviousBlockHash = string.Empty,
            Signature_Key = "Genesis Signature"
        };

        // Вычисляем хэш и ключ подписи для генезис-блока
        genesisBlock.BlockHash = CalculateBlockHash(genesisBlock);

        // Добавляем генезис-блок в блокчейн
        blocks.Add(genesisBlock);

        // Сохраняем генезис-блок в файл
        SaveToFile(genesisBlock);
    }

    public string CalculateBlockHash(Block block)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] blockBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{block.Index}-{block.Timestamp}-{block.Data}-{block.PreviousBlockHash}-{block.Signature_Key}");
            byte[] hashBytes = sha256.ComputeHash(blockBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public byte[] ComputeBlockHash(Block block)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Сериализация блока в JSON
            string blockJson = JsonConvert.SerializeObject(block);

            // Преобразование строки в байты
            byte[] blockBytes = Encoding.UTF8.GetBytes(blockJson);

            // Преобразование строки в байты
            byte[] signatureBytes = StringToByteArray(block.Signature_Key);

            // Добавление байтов подписи к байтам блока перед вычислением хэша
            byte[] dataToHash = new byte[blockBytes.Length + signatureBytes.Length];
            Buffer.BlockCopy(blockBytes, 0, dataToHash, 0, blockBytes.Length);
            Buffer.BlockCopy(signatureBytes, 0, dataToHash, blockBytes.Length, signatureBytes.Length);

            // Вычисление хэша
            return sha256.ComputeHash(dataToHash);
        }
    }

    private byte[] StringToByteArray(string hex)
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

public static class Helper
{
    public static byte[] StringToByteArray(string hex)
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

