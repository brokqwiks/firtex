using Org.BouncyCastle.Crypto;
using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using NBitcoin.RPC;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Sockets;
using System.Net;
using WebSocketSharp.Server;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading.Tasks;
using NBitcoin;

class Program
{
    static void Main(string[] args)
    {
        Blockchain blockchain = new Blockchain();
        blockchain.LoadFromFile();

        SessionNetwork session = new SessionNetwork();
        session.SessionData();
        string address = Components.FindAddressConfig();
        string[] data = Components.ReadWalletData(address);
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Components.ExitSession(sender, eventArgs, session);
        if (Components.ConnectFirtex())
        {   
<<<<<<< HEAD
            Components.StartNodeServer(blockchain, session);
            Components.StartBlocksNode(blockchain, session);
            Components.StartLastBlockNode(blockchain, session);
=======
            Components.StartNodeServer(blockchain);
>>>>>>> c22c8ec6e0cda82e50609bf3d99b271a0cd94ec5
            if (address == null)
            {
                Components.HomeGreeting();
            }
            else
            {
                Components.WalletGreeting(address);

            }
            while (true)
            {
                string user_send = ReadLine();                                                      
                switch (user_send)
                {
                    case "login":
                        if (address == null)
                        {
                            string userSendPrivateKey = ReadLine();
                            Components.LoginInPrivateKey(userSendPrivateKey);
                        }
                        else
                        {
                            Components.SwapWalletMessage();
                            string targetAddress = ReadLine();
                            bool openAddress = Components.FindExeToAddress(targetAddress);
                        }
                        break;

                    case "wallet -c":
                        if (address == null)
                        {
                            Components.CreateWallet();
                        }
                        break;

                    case "send":
                        if (address != null)
                        {
                            Components.CreateTransaction(blockchain, data[1]);
                        }
                        else
                        {
                            break;
                        }

                        break;

                    case "clear -b":
                        blockchain.ClearFiles();
                        break;

                    case "load -b":
                        Components.LoadBlocks(blockchain);
                        break;

                    case "load -bl":
                        Components.GetBalance(blockchain, address);
                        break;

                    case "load -t":
                        Components.TransactionsInfo(blockchain, address);
                        break;

                    case "node -a":
                        Components.NodeActive();
                        break;

                    case "test -r":
                        Components.TestResponse();
                        break;

                    case "get -i":
                        Components.GetIpNetwork();
                        break;

                    case "test -n":
                        Components.TestNodeConnection();
                        break;

                    case "test -lastblock":
                        Components.LastBlockResponce(blockchain);
                        break;

                    case "test -blockchain":
<<<<<<< HEAD
                        Components.AllBlockchainConnection(blockchain);
                        
=======
                        string[] ipAddress = FirtexNetwork.ActiveIpAddressesArray(FirtexNetwork.GetActiveNodes().Result);
                        string activeNode = FirtexNetwork.ConnectionActiveAddresses(ipAddress);
                        string ResponceBlockchain = FirtexNetwork.AllBlockchainNode(activeNode, blockchain);
                        string jsonLocalBlockchain = Blockchain.SerializeBlockchainToJson(blockchain);
                        (bool compare, int differingIndex) = FirtexNetwork.CompareBlockchainsJson(ResponceBlockchain, jsonLocalBlockchain);
                        Console.WriteLine(differingIndex);
                        Console.WriteLine(ResponceBlockchain);
                        FirtexNetwork.UpdateBlockchainFromComparison(jsonLocalBlockchain, ResponceBlockchain, blockchain);
                        List<int> ints = FirtexNetwork.GetExtraBlockIndexes(jsonLocalBlockchain, ResponceBlockchain);
                        List<Block> blocks = FirtexNetwork.OpenExtraBlock(ints, blockchain);
                        List<string> blockJsonArray = new List<string>();
                        foreach (var block in blocks)
                        {
                            string BlockJson = Blockchain.SerializeBlockToJson(block);
                            blockJsonArray.Add(BlockJson);
                        }
                        string combinedJson = Blockchain.CombineBlockJsonArray(blockJsonArray);
                        if(combinedJson != null)
                        {
                            FirtexNetwork.SendExtraBlocks(activeNode, combinedJson);
                        }
>>>>>>> c22c8ec6e0cda82e50609bf3d99b271a0cd94ec5
                        break;

                    case "blockchain -json":
                        string jsonBlockchain = Blockchain.SerializeBlockchainToJson(blockchain);
                        Console.WriteLine(jsonBlockchain);
                        break;
<<<<<<< HEAD

                    case "data":
                        Components.ReadDataFile(session);
                        break;

                    case "start server":
                        Components.StartNodeServer(blockchain, session);
                        break;

                    case "test -data -n":
                        Dictionary<string, Dictionary<string, string>> blockPortsByIp = DataNetwork.GetAllBlockPortsByIp();
                        break;
                    


=======
>>>>>>> c22c8ec6e0cda82e50609bf3d99b271a0cd94ec5
                }
            }
        }
        else
        {
            Components.ErrorConnectFirtexNetwork();
        }
    }

    public static string ReadLine()
    {
        string user_send = Console.ReadLine();
        if (user_send != null)
        {
            return user_send;
        }
        else
        {
            return null;
        }
    }
}


