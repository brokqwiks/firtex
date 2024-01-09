using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Block
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; }
    public string PreviousBlockHash { get; set; }
    public string BlockHash { get; set; }
    public string PublicKey { get; set; }
    public string Signature_Key { get; set; }
    public string AddressSender { get; set; }
    public string AddressToSend { get; set; }
    public string Coins { get; set; }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Timestamp.ToBinary());
        writer.Write(Data);
        writer.Write(PreviousBlockHash);
        writer.Write(BlockHash);
        writer.Write(PublicKey);
        writer.Write(Signature_Key);
        writer.Write(AddressSender);
        writer.Write(AddressToSend);
        writer.Write(Coins);
    }

    public void Deserialize(BinaryReader reader)
    {
        Index = reader.ReadInt32();
        Timestamp = DateTime.FromBinary(reader.ReadInt64());
        Data = reader.ReadString();
        PreviousBlockHash = reader.ReadString();
        BlockHash = reader.ReadString();
        PublicKey = reader.ReadString();
        Signature_Key = reader.ReadString();
        AddressSender = reader.ReadString(); 
        AddressToSend = reader.ReadString();
        Coins = reader.ReadString();
    }

    public string CalculateBlockHash()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] blockBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{Index}-{Timestamp}-{Data}-{PreviousBlockHash}-{Signature_Key}");
            byte[] hashBytes = sha256.ComputeHash(blockBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
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
        Block genesisBlock = new Block
        {
            Index = 0,
            Timestamp = DateTime.Now,
            Data = "Genesis Block",
            PreviousBlockHash = string.Empty,
            Signature_Key = "Genesis Signature",
            PublicKey = "Genesis Key",
            AddressSender = string.Empty,
            AddressToSend = string.Empty,
            Coins = string.Empty,
        };

        genesisBlock.BlockHash = CalculateBlockHash(genesisBlock);

        blocks.Add(genesisBlock);

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
            string blockJson = JsonConvert.SerializeObject(block);

            byte[] blockBytes = Encoding.UTF8.GetBytes(blockJson);

            byte[] signatureBytes = !string.IsNullOrEmpty(block.Signature_Key)
                ? StringToByteArray(block.Signature_Key)
                : new byte[0];

            byte[] dataToHash = new byte[blockBytes.Length + signatureBytes.Length];
            Buffer.BlockCopy(blockBytes, 0, dataToHash, 0, blockBytes.Length);
            if (signatureBytes.Length > 0)
            {
                Buffer.BlockCopy(signatureBytes, 0, dataToHash, blockBytes.Length, signatureBytes.Length);
            }

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

    public static string SerializeBlockchainToJson(Blockchain blockchain)
    {
        try
        {
            JObject blockchainObject = new JObject();

            JArray blocksArray = new JArray();
            foreach (var block in blockchain.blocks)
            {
                JObject blockObject = new JObject();
                blockObject["Index"] = block.Index;
                blockObject["Timestamp"] = block.Timestamp.ToString();
                blockObject["Data"] = block.Data;
                blockObject["PreviousBlockHash"] = block.PreviousBlockHash;
                blockObject["BlockHash"] = block.BlockHash;
                blockObject["PublicKey"] = block.PublicKey;
                blockObject["Signature_Key"] = block.Signature_Key;
                blockObject["AddressSender"] = block.AddressSender;
                blockObject["AddressToSend"] = block.AddressToSend;
                blockObject["Coins"] = block.Coins;

                blocksArray.Add(blockObject);
            }

            blockchainObject["Blocks"] = blocksArray;

            string jsonBlockchain = blockchainObject.ToString();

            return jsonBlockchain;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public static string SerializeBlockToJson(Block block)
    {
        try
        {
            JObject blockObject = new JObject();

            blockObject["Index"] = block.Index;
            blockObject["Timestamp"] = block.Timestamp.ToString();
            blockObject["Data"] = block.Data;
            blockObject["PreviousBlockHash"] = block.PreviousBlockHash;
            blockObject["BlockHash"] = block.BlockHash;
            blockObject["PublicKey"] = block.PublicKey;
            blockObject["Signature_Key"] = block.Signature_Key;
            blockObject["AddressSender"] = block.AddressSender;
            blockObject["AddressToSend"] = block.AddressToSend;
            blockObject["Coins"] = block.Coins;

            string jsonBlock = blockObject.ToString();

            return jsonBlock;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }


    public static Block DeserializeBlock(string blockJson)
    {
        try
        {
            JObject blockObject = JObject.Parse(blockJson);

            Block block = new Block
            {
                Index = (int)blockObject["Index"],
                Timestamp = DateTime.Parse((string)blockObject["Timestamp"]),
                Data = (string)blockObject["Data"],
                PreviousBlockHash = (string)blockObject["PreviousBlockHash"],
                BlockHash = (string)blockObject["BlockHash"],
                PublicKey = (string)blockObject["PublicKey"],
                Signature_Key = (string)blockObject["Signature_Key"],
                AddressSender = (string)blockObject["AddressSender"],
                AddressToSend = (string)blockObject["AddressToSend"],
                Coins = (string)blockObject["Coins"]
            };

            return block;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing block: {ex.Message}");
            return null;
        }
    }

    public static string CombineBlockJsonArray(List<string> blockJsonArray)
    {
        try
        {
            JObject blockchainObject = new JObject();

            JArray blocksArray = new JArray();
            foreach (var blockJson in blockJsonArray)
            {
                JObject blockObject = JObject.Parse(blockJson);
                blocksArray.Add(blockObject);
            }

            blockchainObject["Blocks"] = blocksArray;

            string jsonBlockchain = blockchainObject.ToString();

            return jsonBlockchain;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public static List<Block> DeserializeBlocksFromJson(string jsonBlockchain)
    {
        try
        {
            List<Block> blocks = new List<Block>();

            JObject blockchainObject = JObject.Parse(jsonBlockchain);
            JArray blocksArray = (JArray)blockchainObject["Blocks"];

            foreach (var blockObject in blocksArray)
            {
                Block block = new Block
                {
                    Index = (int)blockObject["Index"],
                    Timestamp = DateTime.Parse((string)blockObject["Timestamp"]),
                    Data = (string)blockObject["Data"],
                    PreviousBlockHash = (string)blockObject["PreviousBlockHash"],
                    BlockHash = (string)blockObject["BlockHash"],
                    PublicKey = (string)blockObject["PublicKey"],
                    Signature_Key = (string)blockObject["Signature_Key"],
                    AddressSender = (string)blockObject["AddressSender"],
                    AddressToSend = (string)blockObject["AddressToSend"],
                    Coins = (string)blockObject["Coins"]
                };

                blocks.Add(block);
            }

            return blocks;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing blocks from JSON: {ex.Message}");
            return null;
        }
    }


    public bool ContainsBlock(Block block)
    {
        // Проверяем, содержится ли блок в локальном блокчейне
        return blocks.Any(b => b.Index == block.Index && b.BlockHash == block.BlockHash);
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

