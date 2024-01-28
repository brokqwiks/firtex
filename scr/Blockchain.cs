using NBitcoin.RPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Blockchain
{
    public List<Block> Blocks = new List<Block>();
    string blockchainFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Firtex/blocks";
    public Block LastBlock;
    public void BlockchainFolder()
    {
        if (!Directory.Exists(blockchainFolder))
        {
            Directory.CreateDirectory(blockchainFolder);
        }
        Directory.CreateDirectory(blockchainFolder);
    }

    public void AddBlock(Block block)
    {
        Blocks.Add(block);
        SaveBlock(block);
    }

    private void SaveBlock(Block block)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open($"{blockchainFolder}/block_{block.Id}", FileMode.Create)))
        {
            block.Serialize(writer);
        }
    }

    public void LoadBlocks()
    {
        if (!Directory.Exists(blockchainFolder))
        {
            Directory.CreateDirectory(blockchainFolder);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(blockchainFolder);
        FileInfo[] files = directoryInfo.GetFiles("*.dat");

        foreach (FileInfo file in files)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(file.FullName)))
            {
                Block block = new Block();
                block.Deserialize(reader);
                Blocks.Add(block);
            }
        }
    }

    public string SerializeBlockJson(Block block)
    {
        try
        {
            JObject blockObject = new JObject();

            blockObject["Id"] = block.Id.ToString();
            blockObject["Time"] = block.Time.ToString();
            blockObject["Type"] = block.Type;
            blockObject["LastBlockHash"] = block.LastBLockHash;
            blockObject["BlockHash"] = block.BlockHash;
            blockObject["PublicKeySender"] = block.PublicKeySender;
            blockObject["SignatureKey"] = block.SignatureKey;
            blockObject["ProofPublicKey"] = block.ProofPublicKey;
            blockObject["ProofSignature"] = block.ProofSignature;
            blockObject["Sender"] = block.Sender;
            blockObject["Recipient"] = block.Recipient;
            blockObject["Token"] = block.Token;
            blockObject["Many"] = block.Many;

            string blockJson = blockObject.ToString();
            return blockJson;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public Block DeserializeBlockJson(string blockJson)
    {
        try
        {
            JObject blockObject = JObject.Parse(blockJson);
            Block block = new Block() 
            {
                Id = (int)blockObject["id"],
                Time = DateTime.Parse((string)blockObject["time"]),
                Type = (string)blockObject["type"],
                LastBLockHash = (string)blockObject["lastblockhash"],
                BlockHash = (string)blockObject["blockhash"],
                PublicKeySender = (string)blockObject["publickeysender"],
                SignatureKey = (string)blockObject["signaturekey"],
                ProofPublicKey = (string)blockObject["proofpublickey"],
                ProofSignature = (string)blockObject["proof"],
                Sender = (string)blockObject["sender"],
                Recipient = (string)blockObject["recipient"],
                Token = (string)blockObject["token"],
                Many = (string)blockObject["many"],
            };
            return block;
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    public Block GetLastBlock()
    {
        try
        {
            if(Blocks.Count > 0) 
            {   
                Block lastblock = Blocks[Blocks.Count - 1];
                LastBlock = lastblock;
                return lastblock;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public List<Block> GetTypeBlock(string type)
    {
        List<Block> stakeBlocks = new List<Block>();

        foreach (var block in Blocks)
        {
            if (block.Type == type)
            {
                stakeBlocks.Add(block);
            }
        }

        return stakeBlocks;
    }

    public string SerializeBlockchainToJson()
    {
        try
        {
            JObject blockchainObject = new JObject();

            JArray blocksArray = new JArray();
            foreach (var block in Blocks)
            {
                JObject blockObject = new JObject();
                blockObject["Id"] = block.Id;
                blockObject["Time"] = block.Time.ToString();
                blockObject["Type"] = block.Type;
                blockObject["LastBlockHash"] = block.LastBLockHash;
                blockObject["BlockHash"] = block.BlockHash;
                blockObject["PublicKeySender"] = block.PublicKeySender;
                blockObject["ProofPublicKey"] = block.ProofPublicKey;
                blockObject["SignatureKey"] = block.SignatureKey;
                blockObject["ProofSignature"] = block.ProofSignature;
                blockObject["Sender"] = block.Sender;
                blockObject["Recipient"] = block.Recipient;
                blockObject["Token"] = block.Token;
                blockObject["Many"] = block.Many;

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

    public string SerializeBlockchain()
    {
        try
        {
            JArray blocksArray = new JArray();

            foreach (var block in Blocks)
            {
                blocksArray.Add(SerializeBlockJson(block));
            }

            JObject blockchainObject = new JObject();
            blockchainObject["Blocks"] = blocksArray;

            return blockchainObject.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serializing blockchain to JSON: {ex.Message}");
            return null;
        }
    }

}
