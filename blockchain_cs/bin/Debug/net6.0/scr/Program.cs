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

class Program
{
    static void Main(string[] args)
    {
        string address = Components.FindAddressConfig();
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
                        string[] WalletData = {mnemonic_phrase, publicKey, generate_address};
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

                        Exe.CreateCopyExe(generate_address, generate_address);
                        Exe.OpenCopyExe(generate_address);
                    }
                }
            }
        }
    }
}
