using Microsoft.Extensions.Logging.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class FirtexConsole
    {
        public static void Start()
        {
            string fileName = WalletApp.GetWalletAddress();
            if (fileName == "Firtex")
            {
                Keys Keys = new Keys();
                Keys.PrivateKey privateKey = new Keys.PrivateKey()
                {
                    wordList = "wordlist/firtex81.txt",
                    mnemonicLenght = 24,
                };
                privateKey.MnemonicPhrase();
                privateKey.GeneratePrivateKeyBytes();
                privateKey.GeneratePrivateKeyHex();

                Keys.PublicKey publicKey = new Keys.PublicKey()
                {
                    PrivateKey = privateKey.privateKeyHex,
                };
                publicKey.GeneratePublicKey();

                byte[] data = Encoding.UTF8.GetBytes("Firtex");

                Keys.Signature signature = new Keys.Signature()
                {
                    Data = data,
                    PrivateKeyBytes = privateKey.privateKeyBytes,
                };

                Keys.Address address = new Keys.Address()
                {
                    publicKeyHex = publicKey.PublicKeyHex,
                };
                address.GenerateAddress();

                signature.SignData();
                signature.VerifySignature(publicKey.PublicKeyBytes);


                DataWallet dataWallet = new DataWallet()
                {
                    Address = address.address,
                    Phrase = privateKey.mnemonicPhrase,
                };
                dataWallet.CreateDataWallet();
                dataWallet.ReadWalletData();

                WalletApp walletApp = new WalletApp()
                {
                    Address = address.address,
                };

                Console.WriteLine(address.address);
                walletApp.CreateWalletApp();
                walletApp.OpenWalletApp();
                Console.ReadLine();
            }
            else
            {
                DataWallet wallet = new DataWallet() 
                {
                    Address = fileName,
                };
            wallet.ReadWalletData();
            Console.WriteLine(wallet.ReadData[0]);
            Blockchain blockchain = new Blockchain();
            blockchain.BlockchainFolder();
            Block block = new Block() 
            {
                Id = 1,
                Time = DateTime.Now,
                Type = "Send",
                Data = "send",
                LastBLockHash = "null",
                BlockHash = "null",
                PublicKeySender = "null",
                SignatureKey = "null",
                ProofPublicKey = "null",
                ProofSignature = "null",
            };
            blockchain.AddBlock(block);
            int[] ports = { 8844, 8845 };
            FirtexNode node = new FirtexNode()
            {
                blockchain = blockchain,
                Data = wallet.ReadData[1],
                ports = ports,
            };

            node.GetLocalIpNetwork();
            node.ConnectFirtexNetwork();
            node.StartServer();
            node.GetActiveNodes();
            foreach (var ip in node.ActiveNodes)
            {
                node.ActiveConnect(ip, 8844);
            }
            Console.ReadLine();
            }

        }
    }
