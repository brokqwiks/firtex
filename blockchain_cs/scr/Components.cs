using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.Arm;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ZeroTier;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using System.Runtime.ConstrainedExecution;

public class Components
{
    public static void HomeGreeting()
    {
        Console.WriteLine("Welcome to Firtex system. Log in to your wallet with a private key or create a new one.");
    }

    public static void WalletGreeting(string address)
    {
        Console.WriteLine($"Welcome to the Firtex system. You are currently in the wallet: {address}");
    }

    public static void SwapWalletMessage()
    {
        Console.WriteLine("You can navigate through an existing wallet on your device by using the address.");
    }


    public static void LoginInPrivateKey(string privateKey)
    {
        string publicKey = PublicKey.GeneratePublicKey(privateKey);
        string address = AddressGenerator.GenerateReadableAddress(publicKey);

        Exe.CreateCopyExe(address, address);
        Exe.OpenCopyExe(address);
    }

    public static string FindAddressConfig()
    {
        string fileName = Exe.GetFileName();
        string address = Config.FindAddressInConfigs(fileName);
        return address;
    }

    public static bool FindExeToAddress(string address)
    {
        return Exe.FindExeToAddress(address);
    }

    public static void CreateWallet()
    {
        string mnemonic_phrase = MnemonicGenerator.generate_phrase();
        if (mnemonic_phrase != null)
        {
            string privateKey = PrivateKey.generate_private_key(mnemonic_phrase);
            if (privateKey != null)
            {
                string publicKey = PublicKey.GeneratePublicKey(privateKey);
                if (publicKey != null)
                {
                    string generate_address = AddressGenerator.GenerateReadableAddress(publicKey);
                    if (generate_address != null)
                    {
                        BinaryFileHandler fileHandler = new BinaryFileHandler();
                        string[] WalletData = { mnemonic_phrase, publicKey, generate_address };
                        Console.WriteLine($"Your mnemonic phrase. Put her in a secret place. If you lose your wallet, you can restore it using this phrase.");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(WalletData[0]);
                        Console.ResetColor();

                        Console.WriteLine();

                        Console.WriteLine($"Your address is Firtex. You can communicate it to everyone. At this address, people will find your wallet and send resources there.");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(WalletData[2]);
                        Console.ResetColor();

                        Console.WriteLine();


                        WalletData walletData = new WalletData
                        {
                            Address = generate_address,
                            PrivateKey = privateKey,
                            PublicKey = publicKey,
                        };

                        fileHandler.RegisterWallet(walletData);

                        Exe.CreateCopyExe(generate_address, generate_address);
                        Console.WriteLine(privateKey);
                        Console.WriteLine(publicKey);
                        Exe.OpenCopyExe(generate_address);
                    }
                }
            }
        }
    }

    public static string[] ReadWalletData(string address)
    {
        string walletFilePath = Path.Combine("wallets", $"{address}.dat");

        if (File.Exists(walletFilePath))
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(walletFilePath)))
                {
                    // Читаем данные кошелька
                    string savedAddress = reader.ReadString();
                    string privateKey = reader.ReadString();
                    string publicKey = reader.ReadString();

                    // Создаем массив строк с данными кошелька
                    string[] walletDataArray = new string[]
                    {
                    savedAddress,
                    privateKey,
                    publicKey,
                    };

                    return walletDataArray;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading wallet data: {ex.Message}");
            }
        }

        return null;
    }

    public static byte[] CreateSignature(string privateKey, string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] privateKeyBytes = PrivateKey.GetPrivateKeyBytes(privateKey);
        byte[] signature = DigitalSignature.SignData(dataBytes, privateKeyBytes);

        return signature;
    }

    public static bool VerifySignature(string publicKey, string data, byte[] signature)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] publicKeyBytes = PublicKey.StringToByteArray(publicKey);
        bool verifed = DigitalSignature.VerifySignature(dataBytes, signature, publicKeyBytes);
        return verifed;
    }

    public static string ConvertSignatureToHex(byte[] signature)
    {
        string signatureHex = DigitalSignature.ConvertSignatureToHex(signature);
        return signatureHex;
    }

    public static void CreateTransaction(Blockchain blockchain, string privateKeyHex)
    {
        string publicKeyHex = PublicKey.GeneratePublicKey(privateKeyHex);
        string address = AddressGenerator.GenerateReadableAddress(publicKeyHex);

        Console.WriteLine("Send the recipient's address.");
        string AddressToSend = Console.ReadLine();
        if (AddressToSend != null)
        {
            Console.WriteLine($"How many coins do you want to send to the address {AddressToSend}?");
            string ManyCoins = Console.ReadLine();
            if (ManyCoins != null)
            {
                Console.WriteLine($"Confirm the transaction of transferring {ManyCoins} coins to the address {AddressToSend}. Send Y if you confirm.");
                if (Console.ReadLine() == "Y")
                {
                    string transactionData = $"{address} sent {ManyCoins} {AddressToSend}";

                    byte[] signature = CreateSignature(privateKeyHex, transactionData);
                    string signatureHex = ConvertSignatureToHex(signature);
                    bool verifySignature = VerifySignature(publicKeyHex, transactionData, signature);
                    if (verifySignature)
                    {
                        Block lastblock = blockchain.GetLastBlock();

                        Block newblock = new Block
                        {
                            Index = lastblock.Index + 1,
                            Timestamp = DateTime.Now,
                            Data = transactionData,
                            PreviousBlockHash = lastblock.BlockHash,
                            PublicKey = publicKeyHex,
                            Signature_Key = signatureHex,
                            AddressSender = address,
                            AddressToSend = AddressToSend,
                            Coins = ManyCoins,

                        };

                        newblock.BlockHash = blockchain.CalculateBlockHash(newblock);
                        blockchain.AddBlock(newblock);
                        Console.WriteLine("The transaction was completed successfully.");
                        Console.WriteLine(verifySignature);
                    }
                    else { Console.WriteLine("Error"); }
                }
            }
        }
    }

    public static void LoadBlocks(Blockchain blockchain)
    {
        foreach (var blk in blockchain.blocks)
        {
            Console.WriteLine($"Block #{blk.Index}");
            Console.WriteLine($"  Timestamp: {blk.Timestamp}");
            Console.WriteLine($"  Data: {blk.Data}");
            Console.WriteLine($"  Previous Block Hash: {blk.PreviousBlockHash}");
            Console.WriteLine($"  Block Hash: {blk.BlockHash}");
            Console.WriteLine($"  Public Key: {blk.PublicKey}");
            Console.WriteLine($"  Signature Key: {blk.Signature_Key}");
            Console.WriteLine($"  AddressSender: {blk.AddressSender}");
            Console.WriteLine($"  AddressToSend: {blk.AddressToSend}");
            Console.WriteLine($"  Coins: {blk.Coins}");
            Console.WriteLine();
        }
    }

    public static void GetBalance(Blockchain blockchain, string address)
    {
        decimal balance = 0;

        foreach (var block in blockchain.blocks)
        {
            if (block.AddressToSend == address)
            {
                string signatureHex = block.Signature_Key;
                byte[] signature = DigitalSignature.HexStringToByteArray(signatureHex);
                bool verifysignature = VerifySignature(block.PublicKey, block.Data, signature);
                if (verifysignature)
                {
                    balance += int.Parse(block.Coins);
                }
            }
            if (block.AddressSender == address)
            {
                string signatureHex = block.Signature_Key;
                byte[] signature = DigitalSignature.HexStringToByteArray(signatureHex);
                bool verifysignature = VerifySignature(block.PublicKey, block.Data, signature);
                if (verifysignature)
                {
                    balance -= int.Parse(block.Coins);
                }
            }
        }
        Console.WriteLine($"Balance for {address}: {balance}");
    }

    public static void TransactionsInfo(Blockchain blockchain, string address)
    {
        int transactionIndex = 0;
        foreach (var block in blockchain.blocks)
        {
            if (block.AddressToSend == address)
            {
                transactionIndex++;
                 Console.WriteLine($"Transaction: {transactionIndex}");
                   Console.WriteLine($"{block.AddressSender} -> {block.AddressToSend}");
                Console.WriteLine($"+ {block.Coins}");
            }
            if (block.AddressSender == address)
            {
               transactionIndex++;
                Console.WriteLine();
                Console.WriteLine($"Transaction: {transactionIndex}");
                Console.WriteLine($"{block.AddressSender} -> {block.AddressToSend}");
                Console.WriteLine($"- {block.Coins} Firtex Coins");
                Console.WriteLine();
             }
        }
    }

    public static bool ConnectFirtex()
    {
        if (FirtexNetwork.ConnectFirtexNetwork())
        {
            Console.WriteLine("Successful network connection.");
            return true;
        }
        else
        {
            Console.WriteLine("Couldn't connect to the network");
            return false;
        }
    }

    public static void ErrorConnectFirtexNetwork()
    {
         Console.WriteLine("An error occurred while connecting to the blockchain network. Try again by restarting the application.");
    }

    public static List<string> GetActiveNodeIPAddresses(string json)
    {
        JArray nodes = JArray.Parse(json);
        if (nodes != null)
        {
            List<string> ipAddresses = nodes
                .Where(node => node["lastOnline"] != null)
                .SelectMany(node => node["config"]["ipAssignments"]
                    .Values<string>()
                    .Where(ip => ip.StartsWith("192.168.192.")))
                .ToList();

            return ipAddresses;
        }
        return null;
    }

    public static void GetIpNetwork()
    {   
        string testResponce = FirtexNetwork.TestResponceNetwork().Result;
        string localIP = FirtexNetwork.GetLocalIpNetwork();
        Console.WriteLine($"{localIP}");
    }

    public static void StartNodeServer(Blockchain blockchain, SessionNetwork session)
    {
        string IpAddress = FirtexNetwork.GetLocalIpNetwork();
        Task.Run(() => ServerFirtexNetwork.StartServer(IpAddress, blockchain, session, "Main"));

    }

    public static void StartBlocksNode(Blockchain blockchain, SessionNetwork session)
    {
        string IpAddress = FirtexNetwork.GetLocalIpNetwork();
        Task.Run(() => ServerFirtexNetwork.StartServer(IpAddress, blockchain, session, "Block"));
    }

    public static void StartLastBlockNode(Blockchain blockchain, SessionNetwork session)
    {
        string IpAddress = FirtexNetwork.GetLocalIpNetwork();
        Task.Run(() => ServerFirtexNetwork.StartServer(IpAddress, blockchain, session, "LastBlock"));
    }

    public static void StartAllBlockchainNode(Blockchain blockchain, SessionNetwork session)
    {
        string IpAddress = FirtexNetwork.GetLocalIpNetwork();
        Task.Run(() => ServerFirtexNetwork.StartServer(IpAddress, blockchain, session, "AllBlockchain"));
    }

    public static void TestNodeConnection(SessionNetwork session)
    {
        string activesnodes = FirtexNetwork.GetActiveNodes().Result;
        string[] activenodeArr = FirtexNetwork.ActiveIpAddressesArray(activesnodes);
        List<string> activenode = FirtexNetwork.ConnectionActiveAddresses(activenodeArr, session);
        foreach (var ipaddress in activenode)
        {
            FirtexNetwork.GetPortsConnectionServer(ipaddress, 8844, session);
        }
    }

    public static void NodeActive()
    {
        string activeNodesResult = FirtexNetwork.GetActiveNodes().Result;
        List<string> ipAddresses = Components.GetActiveNodeIPAddresses(activeNodesResult);

        Console.WriteLine("IP addresses of active nodes:");
        foreach (var ipAddress in ipAddresses)
        {
            Console.WriteLine(ipAddress);
        }
    }

    public static void TestResponse()
    {
        string testResponce = FirtexNetwork.TestResponceNetwork().Result;
        Console.WriteLine(testResponce);
    }

    public static void LastBlockResponce(Blockchain blockchain, SessionNetwork session) 
    {
        string[] activeNodes = FirtexNetwork.ActiveIpAddressesArray(FirtexNetwork.GetActiveNodes().Result);
        List<string> IpAddressNode = FirtexNetwork.ConnectionActiveAddresses(activeNodes, session);
        Dictionary<string, Dictionary<string, string>> NodePorts = DataNetwork.GetAllBlockPortsByIp();
        foreach (var ipaddress in IpAddressNode)
        {
            int Port = Int32.Parse(NodePorts[ipaddress]["LastBlockPort"]);
            bool LastBlockResponce = FirtexNetwork.GetLastBlockNode(ipaddress, blockchain, Port);
            Console.WriteLine(LastBlockResponce);
        }

    }

    public static void ExitSession(object sender, EventArgs e, SessionNetwork session) 
    {
        session.ClearData();
    }

    public static void AllBlockchainConnection(Blockchain blockchain, SessionNetwork session)
    {
        string[] ipAddress = FirtexNetwork.ActiveIpAddressesArray(FirtexNetwork.GetActiveNodes().Result);
        List<string> activeNode = FirtexNetwork.ConnectionActiveAddresses(ipAddress, session);
        foreach (var ipaddress in activeNode)
        {
            Dictionary<string, Dictionary<string, string>> NodePorts = DataNetwork.GetAllBlockPortsByIp();
            int Port = Int32.Parse(NodePorts[ipaddress]["MainPort"]);
            string ResponceBlockchain = FirtexNetwork.AllBlockchainNode(ipaddress, blockchain, Port);
            string jsonLocalBlockchain = Blockchain.SerializeBlockchainToJson(blockchain);
            FirtexNetwork.AllBlockchainNode(ipaddress, blockchain, Port);
        }

    }

    public static void ReadDataFile(SessionNetwork session)
    {
        List<string> dataSession = session.ReadData();
        foreach (var item in dataSession)
        {
            Console.WriteLine(item);
        }
    }

    public static void SyncFirtexNetwork(Blockchain blockchain, SessionNetwork session)
    {
        string[] ipAddress = FirtexNetwork.ActiveIpAddressesArray(FirtexNetwork.GetActiveNodes().Result);
        List<string> activeNode = FirtexNetwork.ConnectionActiveAddresses(ipAddress, session);
        if (blockchain.blocks.Count != 0)
        {
            foreach (var ipaddress in activeNode)
            {
                Dictionary<string, Dictionary<string, string>> NodePorts = DataNetwork.GetAllBlockPortsByIp();
                int LastBlockPort = Int32.Parse(NodePorts[ipaddress]["LastBlockPort"]);
                bool LastBlockResponce = FirtexNetwork.GetLastBlockNode(ipaddress, blockchain, LastBlockPort);
                if (!LastBlockResponce)
                {
                    string blockchainJson = ServerFirtexNetwork.ReturnAllBLockchainResponce(blockchain).Result;
                    var blocks = Blockchain.DeserializeBlocksFromJson(blockchainJson);
                    FirtexNetwork.SendBlockchainBlocks(ipaddress, blocks, blockchain, Int32.Parse(NodePorts[ipaddress]["BlockPort"]));
                }
                continue;
            }
        }
        else
        {
            Console.WriteLine("Blockchain is null");
        }
    }

    public static void IsBlockchainFolderExists(string folderPath)
    {   
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    

}

