using Microsoft.Extensions.Logging.Internal;
using NBitcoin.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Keys;
using static System.Collections.Specialized.BitVector32;

public class FirtexConsole
{
    public static void Start()
    {
        Blockchain blockchain = new Blockchain();
        blockchain.LoadBlocks();

        string fileName = WalletApp.GetWalletAddress();
        if (fileName == "Firtex")
        {
            while (true)
            {
                string read = Console.ReadLine();
                switch (read)
                {
                    case "create":
                        Keys.PrivateKey privateKey = new Keys.PrivateKey()
                        {
                            mnemonicLenght = 24,
                            wordList = "wordlist/firtex81.txt",
                        };
                        privateKey.MnemonicPhrase();
                        privateKey.GeneratePrivateKeyBytes();
                        privateKey.GeneratePrivateKeyHex();

                        Keys.PublicKey publicKey = new Keys.PublicKey()
                        {
                            PrivateKey = privateKey.privateKeyHex,
                        };
                        publicKey.GeneratePublicKey();

                        Keys.Address address = new Keys.Address()
                        {
                            publicKeyHex = publicKey.PublicKeyHex
                        };
                        address.GenerateAddress();

                        DataWallet dataWallet = new DataWallet()
                        {
                            Address = address.address,
                            Phrase = privateKey.mnemonicPhrase,
                        };
                        dataWallet.CreateDataWallet();

                        WalletApp wallet = new WalletApp()
                        {
                            Address = address.address,
                        };
                        Console.WriteLine(privateKey.mnemonicPhrase);
                        Console.WriteLine(address.address);
                        wallet.CreateWalletApp();
                        wallet.OpenWalletApp();
                        break;

                    case "login":
                        LoginPhrase(Console.ReadLine());
                        break;
                }
            }
        }
        else
        {   
            DataWallet dataWallet = new DataWallet()
            {
                Address = fileName,
            };
            dataWallet.ReadWalletData();

            FirtexNode node = new FirtexNode()
            {
                blockchain = blockchain,
                Data = dataWallet.ReadData[1],
            };

            node.ConnectFirtexNetwork();
            Task.Run(() => node.StartServer(8844));
            Task.Run(() => node.StartServer(8845));

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            var timer = new System.Threading.Timer((e) =>
            {
                node.GetActiveNodes();
                node.Connect(8844);
            }, null, startTimeSpan, periodTimeSpan);

            Console.WriteLine(dataWallet.ReadData[0]);
            Console.WriteLine(dataWallet.ReadData[1]);
            Console.ReadLine();
        }
    }

    private static string LoginPhrase(string phrase)
    {
        Keys.PrivateKey privateKey = new Keys.PrivateKey()
        {
            mnemonicPhrase = phrase,
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

        DataWallet dataWallet = new DataWallet()
        {
            Address = address.address,
        };
        dataWallet.CreateDataWallet();

        WalletApp wallet = new WalletApp() 
        {
            Address = address.address,
        };
        wallet.CreateWalletApp();
        wallet.OpenWalletApp();
        return address.address;
    }
}