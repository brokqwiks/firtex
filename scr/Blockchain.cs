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
    string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    string blockchainFolder;
    public Block LastBlock;
    public void BlockchainFolder()
    {
        string BlockchainFolder = Path.Combine(appdata, "Firtex");
        if (!Directory.Exists(BlockchainFolder))
        {
            Directory.CreateDirectory(BlockchainFolder);
        }
        Directory.CreateDirectory($"{BlockchainFolder}/blocks");
        blockchainFolder = $"{BlockchainFolder}/blocks";
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

    public string SerializeBlockJson(Block block)
    {
        try
        {
            JObject blockObject = new JObject();

            blockObject["Id"] = block.Id.ToString();
            blockObject["Time"] = block.Time.ToString();
            blockObject["Type"] = block.Type;
            blockObject["Data"] = block.Data;
            blockObject["LastBlockHash"] = block.LastBLockHash;
            blockObject["BlockHash"] = block.BlockHash;
            blockObject["PublicKeySender"] = block.PublicKeySender;
            blockObject["SignatureKey"] = block.SignatureKey;
            blockObject["ProofPublicKey"] = block.ProofPublicKey;
            blockObject["ProofSignature"] = block.ProofSignature;

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
                Data = (string)blockObject["data"],
                LastBLockHash = (string)blockObject["lastblockhash"],
                BlockHash = (string)blockObject["blockhash"],
                PublicKeySender = (string)blockObject["publickeysender"],
                SignatureKey = (string)blockObject["signaturekey"],
                ProofPublicKey = (string)blockObject["proofpublickey"],
                ProofSignature = (string)blockObject["proof"],
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
}
