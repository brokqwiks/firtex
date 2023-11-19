using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class Block
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; }
    public string PreviousBlockHash { get; set; }
    public string BlockHash { get; set; }
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
}

public class Blockchain
{
    public List<Block> blocks = new List<Block>();
    private string filePath = "blockchain.dat";

    public void AddBlock(Block block)
    {
        blocks.Add(block);
        SaveToFile();
    }

    public void LoadFromFile()
    {
        if (File.Exists(filePath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    Block block = new Block();
                    block.Deserialize(reader);
                    blocks.Add(block);
                }
            }
        }
        else
        {
            // Если файл не существует, создаем генезис-блок
            CreateGenesisBlock();
        }
    }

    public void SaveToFile()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (Block block in blocks)
            {
                block.Serialize(writer);
            }
        }
    }

    public void ClearFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
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

        // Сохраняем блокчейн в файл
        SaveToFile();
    }

    public string CalculateBlockHash(Block block)
    {
        // Вы можете использовать любой алгоритм хэширования, например, SHA-256
        // Здесь представлен пример с SHA-256
        using (SHA256 sha256 = SHA256.Create())
        {
            // Конвертируем блок в массив байтов для хэширования
            byte[] blockBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{block.Index}-{block.Timestamp}-{block.Data}-{block.PreviousBlockHash}-{block.Signature_Key}");

            // Вычисляем хэш
            byte[] hashBytes = sha256.ComputeHash(blockBytes);

            // Преобразуем хэш в строку
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
