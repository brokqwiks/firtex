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

class Program
{
    static void Main(string[] args)
    {   
        Blockchain blockchain = new Blockchain();
        blockchain.LoadFromFile();

        string address = Components.FindAddressConfig();
        string[] data = Components.ReadWalletData(address);
        if (address == null )
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
                    if(address == null)
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

                case "create wallet":
                    if(address == null)
                    {
                        Components.CreateWallet();
                    }
                    break;

                case "send":
                    Components.CreateTransaction(blockchain, data[1]);

                    break;

                case "clear blocks":
                    blockchain.ClearFiles();
                    break;

                case "load blocks":
                    Components.LoadBlocks(blockchain);
                    break;

                case "balance":
                    Components.GetBalance(blockchain, address);
                    break;
            }
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


