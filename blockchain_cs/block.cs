using System;
using System.Collections.Generic;
using System.IO;

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
    }

    public void Deserialize(BinaryReader reader)
    {
        Index = reader.ReadInt32();
        Timestamp = DateTime.FromBinary(reader.ReadInt64());
        Data = reader.ReadString();
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


   public void ClearFile() { 
       File.WriteAllText(filePath, string.Empty);
   }
   
}
