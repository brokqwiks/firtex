using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProofOfStake
{
    public Blockchain blockchain { get; set; }
    public string Data { get; set; }
    public string Validator;

    public string GetValidator()
    {
        Dictionary<string, decimal> stakerBalances = new Dictionary<string, decimal>();

        List<Block> stakeBlocks = blockchain.GetTypeBlock("Stake");

        foreach (Block block in stakeBlocks)
        {
            if (!string.IsNullOrEmpty(block.Sender))
            {
                if (!string.IsNullOrEmpty(block.Many) && decimal.TryParse(block.Many, out decimal coins))
                {
                    if (stakerBalances.ContainsKey(block.Sender))
                    {
                        stakerBalances[block.Sender] += coins;
                    }
                    else
                    {
                        stakerBalances[block.Sender] = coins;
                    }
                }
            }
        }
        string topStaker = stakerBalances.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        Validator = topStaker;
        return topStaker;
    }

    public void PutStake(string amount)
    {
        GetValidator();
        Transaction transaction = new Transaction()
        {
            blockchain = blockchain,
            Data = Data,
            Type = "Stake",
            Recipient = "15iWKLBtpsSodvGp5Ykcbfq7agafB45B8qwrP4tRHVG2pAznhvh",
            Many = amount,
            Token = "Firtex",
        };
        Block block = transaction.CreateTransaction();
        if (Validator == null)
        {
            block.ProofSignature = block.SignatureKey;
            block.ProofPublicKey = block.PublicKeySender;
            block.BlockHash = block.CalculateBlockHash();
            block.LastBLockHash = blockchain.GetLastBlock().CalculateBlockHash();
            block.Id = blockchain.GetLastBlock().Id;
            blockchain.AddBlock(block);
        }
    }
}