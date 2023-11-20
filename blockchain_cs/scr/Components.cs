using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;

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

                        byte[] privateKeyBytes = PrivateKey.GeneratePrivateKeyBytes(privateKey);
                        byte[] addressBytes = Encoding.UTF8.GetBytes(generate_address);

                        using (SHA256 sha256 = SHA256.Create())
                        {
                            byte[] hash = sha256.ComputeHash(addressBytes);

                            // Подпишите хэш
                            byte[] signature = DigitalSignature.SignData(hash, privateKeyBytes);
                            string signatureHex = DigitalSignature.ConvertSignatureToHex(signature);

                            WalletData walletData = new WalletData
                            {
                                Address = generate_address,
                                PrivateKey = privateKey,
                                PublicKey = publicKey,
                                SignatureKey = signatureHex,
                            };

                            fileHandler.RegisterWallet(walletData);

                        }

                        Exe.CreateCopyExe(generate_address, generate_address);
                        Exe.OpenCopyExe(generate_address);
                    }
                }
            }
        }
    }

    public static string[] ReadWalletData(string address)
    {
        BinaryFileHandler fileHandler = new BinaryFileHandler();
        Dictionary<string, string> readData = fileHandler.ReadWalletData(address);

        if (readData != null)
        {
            Console.WriteLine("Wallet Data Read Successfully");

            // Создаем массив строк для хранения данных
            List<string> result = new List<string>();

            foreach (var entry in readData)
            {
                // Добавляем каждую пару ключ-значение в массив строк
                result.Add($"{entry.Value}");
            }

            // Возвращаем массив строк
            return result.ToArray();
        }
        else
        {
            // Возвращаем пустой массив строк в случае ошибки
            return new string[0];
        }
    }
    
    public static byte[] CreateSignatureBytes(string data, string privateKeyHex)
    {
        byte[] privateKeyBytes = PrivateKey.GeneratePrivateKeyBytes(privateKeyHex);
        byte[] publicKeyBytes = PublicKey.GeneratePublicKeyBytes(privateKeyBytes);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);    
        byte[] signatureBytes = DigitalSignature.SignData(dataBytes, privateKeyBytes);

        return signatureBytes;
    }

    public static string ConvertSignatureToHex(byte[] SignatureBytes)
    {
        if (SignatureBytes != null)
        {
            return DigitalSignature.ConvertSignatureToHex(SignatureBytes);
        }
        return null;
    }

    public static bool VerifySignatureBytes(string data, byte[] signatureBytes, string publicKey)
    {
        byte[] publicKeyBytes = PublicKey.StringToByteArray(publicKey);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        bool verifed = DigitalSignature.VerifySignature(dataBytes, signatureBytes, publicKeyBytes);

        return verifed;
    }

    public static bool VerifySignatureHex(string data, string signatureHex, string publicKey)
    {
        byte[] publicKeyBytes = PublicKey.StringToByteArray(publicKey);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] signatureBytes = DigitalSignature.HexStringToByteArray(signatureHex);

        bool verifed = DigitalSignature.VerifySignature(dataBytes, signatureBytes, publicKeyBytes);

        return verifed;
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
            if( ManyCoins != null )
            {
                Console.WriteLine($"Confirm the transaction of transferring {ManyCoins} coins to the address {AddressToSend}. Send Y if you confirm.");
                if(Console.ReadLine() == "Y")
                {
                    string transactionData = $"{address} sent {ManyCoins} coins to {AddressToSend}";

                    byte[] signatureBytes = CreateSignatureBytes(transactionData, privateKeyHex);
                    string signatureHex = ConvertSignatureToHex(signatureBytes);

                    Block lastBlock = blockchain.GetLastBlock();

                    Block newBlock = new Block
                     {
                         Index = lastBlock.Index + 1,
                         Timestamp = DateTime.Now,
                         Data = transactionData,
                         PreviousBlockHash = lastBlock.BlockHash,
                         PublicKey = publicKeyHex,
                         Signature_Key = signatureHex // Замените на ваш приватный ключ
                     };

                      newBlock.BlockHash = blockchain.CalculateBlockHash(newBlock);

                      // Добавление нового блока в блокчейн
                      blockchain.AddBlock(newBlock);

                     Console.WriteLine("The transaction was completed successfully.");
                    }
                }
                else
                {
                    Console.WriteLine("The transaction was successfully canceled.");
                    return;
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
            Console.WriteLine();
        }
    }
}