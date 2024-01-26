using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class Block
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public string LastBLockHash { get; set; }
    public string BlockHash { get; set; }
    public string PublicKeySender { get; set; }
    public string SignatureKey { get; set; }
    public string ProofSignature { get; set; }
    public string ProofPublicKey { get; set; }

    public void Serialize(BinaryWriter writer)
    {
        try
        {
            writer.Write(Id);
            writer.Write(Time.ToBinary());
            writer.Write(Type);
            writer.Write(Data);
            writer.Write(LastBLockHash);
            writer.Write(BlockHash);
            writer.Write(PublicKeySender);
            writer.Write(SignatureKey);
            writer.Write(ProofPublicKey);
            writer.Write(ProofPublicKey);

        }
        catch (Exception ex)
        {

        }
    }

    public void Deserialize(BinaryReader reader)
    {
        Id = reader.ReadInt32();
        Time = DateTime.FromBinary(reader.ReadInt64());
        Type = reader.ReadString();
        Data = reader.ReadString();
        LastBLockHash = reader.ReadString();
        BlockHash = reader.ReadString();
        PublicKeySender = reader.ReadString();
        SignatureKey = reader.ReadString();
        ProofSignature = reader.ReadString();
        ProofPublicKey = reader.ReadString();
    }
    
    public string CalculateBlockHash()
    {
        using(SHA256 sha256 = SHA256.Create())
        {
            byte[] blockBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{Id}-{Time}-{Data}-{LastBLockHash}-{SignatureKey}");
            byte[] hashBytes = sha256.ComputeHash(blockBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}