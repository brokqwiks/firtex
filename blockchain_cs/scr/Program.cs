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

        string address = Components.FindAddressConfig();
        string[] data = Components.ReadWalletData(address);
        if (Components.ConnectFirtex())
        {   
            Components.StartNodeServer();
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
                        Components.CreateTransaction(blockchain, data[1]);

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


