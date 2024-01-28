using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Transaction
{
    public Blockchain blockchain { get; set; }
    public string Data { get; set; }
    public string Token { get; set; }
    public string Recipient { get; set; }
    public string Type { get; set; }
    public string Many { get; set; }

    public Block CreateTransaction()
    {
        try
        {
            Keys.PrivateKey privateKey = new Keys.PrivateKey()
            {
                mnemonicPhrase = Data,
            };
            privateKey.GeneratePrivateKeyHex();
            privateKey.GeneratePrivateKeyBytes();

            Keys.PublicKey publicKey = new Keys.PublicKey()
            {
                PrivateKey = privateKey.privateKeyHex,
            };
            publicKey.GeneratePublicKey();

            Keys.Address address = new Keys.Address()
            {
                publicKeyHex = publicKey.PublicKeyHex,
            };
            address.GenerateAddress();

            Block block = new Block()
            {
                Time = DateTime.Now,
                Type = Type,
                Sender = address.address,
                PublicKeySender = publicKey.PublicKeyHex,
                Recipient = Recipient,
                Token = Token,
                Many = Many,
            };
            byte[] transactionData = block.SerializeData();
            Keys.Signature signature = new Keys.Signature()
            {
                Data = transactionData,
                PrivateKeyBytes = privateKey.privateKeyBytes,
            };
            signature.SignData();
            signature.VerifySignature(publicKey.PublicKeyBytes);
            if (signature.Verify)
            {
                signature.ConvertSignatureToHex(signature.SignatureKey);
                block.SignatureKey = signature.SignatureHex;
                return block;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex) {
            return null;
        }
    }
}
