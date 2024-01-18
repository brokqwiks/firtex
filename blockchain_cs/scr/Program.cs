using Org.BouncyCastle.Crypto;
using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using System.Timers;
using System;
using System.ComponentModel;

class Program
{
    static void Main(string[] args)
    {
        Blockchain blockchain = new Blockchain();
        Components.IsBlockchainFolderExists("blockchain_blocks");
        blockchain.LoadFromFile();

        SessionNetwork session = new SessionNetwork();
        session.SessionData();
        string address = Components.FindAddressConfig();
        string[] data = Components.ReadWalletData(address);
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Components.ExitSession(sender, eventArgs, session);
        if (Components.ConnectFirtex())
        {   
            Components.StartNodeServer(blockchain, session);
            Components.StartBlocksNode(blockchain, session);
            Components.StartLastBlockNode(blockchain, session);
            Components.StartAllBlockchainNode(blockchain, session);

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            var timer = new System.Threading.Timer((e) =>
            {
                Components.SyncFirtexNetwork(blockchain, session);
            }, null, startTimeSpan, periodTimeSpan);
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
                            Components.LoginPhrase();
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
                        Components.TestNodeConnection(session);
                        break;

                    case "test -lastblock":
                        Components.LastBlockResponce(blockchain, session);
                        break;

                    case "blockchain -json":
                        string jsonBlockchain = Blockchain.SerializeBlockchainToJson(blockchain);
                        Console.WriteLine(jsonBlockchain);
                        break;

                    case "data":
                        Components.ReadDataFile(session);
                        Dictionary<string, string> SessionData =  session.ReadNodeData();
                        Console.WriteLine(SessionData["Main"]);
                        break;

                    case "start server":
                        Components.StartNodeServer(blockchain, session);
                        break;

                    case "test -data -n":
                        Dictionary<string, Dictionary<string, string>> blockPortsByIp = DataNetwork.GetAllBlockPortsByIp();
                        break;

                    case "sync":
                        Components.SyncFirtexNetwork(blockchain, session);
                        break;

                    case "block -c -g":
                        blockchain.CreateGenesisBlock();
                        break;
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


