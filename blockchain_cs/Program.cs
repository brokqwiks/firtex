using Org.BouncyCastle.Crypto;
using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using NBitcoin.RPC;

class Program
{
    static void Main()
    {
        Blockchain blockchain = new Blockchain();
        BinaryFileHandler fileHandler = new BinaryFileHandler();
        Dictionary<string, string> WalletData = fileHandler.ReadWalletData();
        if (WalletData != null)
        {
            string address = WalletData["Address"];
            Console.WriteLine("Welcome to the Firtex system! You are already logged in to our system, here is your address: " + address);
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Welcome to the Firtex system! In order to use our resources to the fullest, then create your wallet.");
            Console.WriteLine();
        }
        while (true)
        {
            string send_user = Console.ReadLine();
            if (send_user != null)
                    switch (send_user)
                    {
                        case "create wallet":

                            // Чтение данных из файла и вывод в консоль

                            string signatureHex = null; // Объявляем переменную здесь, чтобы она была видна за пределами блока using

                            if (WalletData == null)
                            {
                                string mnemonic_phrase = MnemonicGenerator.generate_phrase();
                                string privateKeyHex = PrivateKey.generate_private_key(mnemonic_phrase);
                                string publicKeyHex = PublicKey.GeneratePublicKey(privateKeyHex);
                                string address = AddressGenerator.GenerateReadableAddress(publicKeyHex);
                                byte[] privateKeyBytes = PrivateKey.GeneratePrivateKeyBytes(mnemonic_phrase);
                                byte[] publicKeyBytes = PublicKey.GeneratePublicKeyBytes(privateKeyBytes);

                                using (SHA256 sha256 = SHA256.Create())
                                {
                                    byte[] hashedData = sha256.ComputeHash(privateKeyBytes);

                                    // Подпись хэшированных данных
                                    byte[] signatureBytes = DigitalSignature.SignData(hashedData, privateKeyBytes);
                                    signatureHex = DigitalSignature.ConvertSignatureToHex(signatureBytes);
                                }

                                WalletData walletData = new WalletData()
                                {
                                    Address = address,
                                    PrivateKey = privateKeyHex,
                                    PublicKey = publicKeyHex,
                                    SignatureKey = signatureHex,
                                };

                                fileHandler.RegisterWallet(walletData);

                                Console.WriteLine("This is your private key. Do not tell anyone to him and keep it in a well-protected place: " + privateKeyHex);
                                Console.WriteLine();
                                Console.WriteLine("This is the public key of your wallet. It can be shared with other people: " + publicKeyHex);
                                Console.WriteLine();
                                Console.WriteLine("This is your address, you can share it with other users to receive coins: " + address);
                                Console.WriteLine();
                                Console.WriteLine("This is a unique electronic signature of your wallet, proving your ownership of the wallet and funds: " + signatureHex);

                            Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("You are already logged in to the system, if you want to create a new wallet, then log out.");
                                Console.WriteLine();

                            }

                        break;


                        case "address":
                            Console.WriteLine("To get the address, send the public key of your or someone else's wallet.");
                            string publicKey_userSend = Console.ReadLine();
                            string addressSend = AddressGenerator.GenerateReadableAddress(publicKey_userSend);
                            Console.WriteLine();
                            Console.WriteLine("Address: " + addressSend);
                        
                            break;

                        case "load block":
                            blockchain.LoadFromFile();


                            Block lastBlock = blockchain.GetLastBlock();

                            if (lastBlock != null)
                            {
                                Dictionary<string, string> blockInfo = new Dictionary<string, string>
                            {
                                { "Index", lastBlock.Index.ToString() },
                                { "Timestamp", lastBlock.Timestamp.ToString() },
                                { "Data", lastBlock.Data },
                                { "Previous Block Hash", lastBlock.PreviousBlockHash },
                                { "Block Hash", lastBlock.BlockHash },
                                { "Signature Key", lastBlock.Signature_Key }
                            };

                                foreach (var kvp in blockInfo)
                                {
                                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Blockchain is empty");
                            }

                            break;

                        case "delete blockchain":
                            Blockchain blockchain1 = new Blockchain();
                            blockchain1.ClearFile();
                            break;

                        case "create block":
                            //Empty so far
                            break;

                        case "exit":
                            fileHandler.ClearFile();
                            Console.WriteLine("The logout was completed successfully");
                            Console.WriteLine();
                            break;


                    }
                }
            }
        }
