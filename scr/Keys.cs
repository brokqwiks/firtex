using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using NBitcoin;
using System.Numerics;
using NBitcoin.Crypto;

public class Keys
{
    public class PrivateKey
    {
        public string wordList { get; set; }
        public int mnemonicLenght { get; set; }
        public byte[] privateKeyBytes;
        public string privateKeyHex;
        public string mnemonicPhrase;
        public string MnemonicPhrase()
        {
            string[] words = File.ReadAllLines(wordList);
            Random random = new Random();
            string[] mnemonicWords = new string[mnemonicLenght];

            for (int word = 0; word < mnemonicLenght; word++)
            {
                int randomIndex = random.Next(0, words.Length);
                mnemonicWords[word] = words[randomIndex];
            }

            string mnemonicphrase = string.Join(" ", mnemonicWords);
            mnemonicPhrase = mnemonicphrase;
            return mnemonicphrase;
        }

        public byte[] GeneratePrivateKeyBytes()
        {
            if (mnemonicPhrase != null)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] seed = Encoding.UTF8.GetBytes(mnemonicPhrase);
                    byte[] privateKey = sha256.ComputeHash(seed);
                    privateKeyBytes = privateKey;
                    return privateKey;
                }
            }
            else
            {
                return null;
            }
        }

        public string GeneratePrivateKeyHex()
        {
            byte[] mnemonicBytes = Encoding.UTF8.GetBytes(mnemonicPhrase);
            string private_key = Sha256.ComputeSHA256Hash(mnemonicPhrase);
            bool validate_private_key = false;

            if (ValidatePrivateKey(private_key))
            {
                privateKeyHex = private_key;
                return private_key;
            }
            else
            {
                while (validate_private_key == false)
                {
                    return GeneratePrivateKeyHex();
                }
                return private_key;
            }
        }

        public bool ValidatePrivateKey(string private_key)
        {
            string maxPrivateKeyHex = "FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFE BAAEDCE6 AF48A03B BFD25E8C D0364140";
            return String.Compare(private_key, maxPrivateKeyHex, StringComparison.OrdinalIgnoreCase) <= 0;
        }
    }

    public class PublicKey
    {
        public string PrivateKey { get; set; }
        public byte[] PublicKeyBytes;
        public string PublicKeyHex;

        public string GeneratePublicKey()
        {
            try
            {
                byte[] privateKeyBytes = StringToByteArray(PrivateKey);

                Key privateKey = new Key(privateKeyBytes);

                byte[] publicKeyBytes = privateKey.PubKey.ToBytes();
                string publicKeyHex = ByteArrayToString(publicKeyBytes);
                PublicKeyBytes = publicKeyBytes;
                PublicKeyHex = publicKeyHex;
                return publicKeyHex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private static string ByteArrayToString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            if (hex != null)
            {
                int numberChars = hex.Length;
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
                return bytes;
            }
            return null;
        }
    }

    public class Address
    {   
        public string publicKeyHex {  get; set; }
        public string address;
        public string GenerateAddress()
        {
            try
            {
                byte[] hashBytes = StringToByteArray(publicKeyHex);

                byte prefix = 0x00;

                byte[] versionedBytes = new byte[hashBytes.Length + 1];
                versionedBytes[0] = prefix;
                Array.Copy(hashBytes, 0, versionedBytes, 1, hashBytes.Length);

                byte[] checksum = CalculateChecksum(versionedBytes);

                byte[] totalBytes = new byte[versionedBytes.Length + checksum.Length];
                Array.Copy(versionedBytes, 0, totalBytes, 0, versionedBytes.Length);
                Array.Copy(checksum, 0, totalBytes, versionedBytes.Length, checksum.Length);

                string base58CheckAddress = Base58CheckEncode(totalBytes);
                address = base58CheckAddress;

                return base58CheckAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private byte[] CalculateChecksum(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash1 = sha256.ComputeHash(data);
                byte[] hash2 = sha256.ComputeHash(hash1);

                hash2[0] = 0x8F;

                byte[] checksum = new byte[4];
                Array.Copy(hash2, checksum, 4);

                return checksum;
            }
        }


        private string Base58CheckEncode(byte[] data)
        {
            const string base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder();

            BigInteger value = new BigInteger(data.Reverse().ToArray());

            while (value > 0)
            {
                result.Insert(0, base58Chars[(int)(value % 58)]);
                value /= 58;
            }

            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result.Insert(0, '1');
            }

            return result.ToString();
        }
    }

    public class Signature
    {
        public byte[] Data;
        public byte[] PrivateKeyBytes;
        public byte[] SignatureKey;
        public string SignatureHex;
        public bool Verify;
        public byte[] SignData()
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Data);

                    var key = new Key(PrivateKeyBytes);
                    var signature = key.Sign(new uint256(hash), false);
                    SignatureKey = signature.ToDER();
                    return signature.ToDER();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing data: {ex.Message}");
                return null;
            }
        }

        public bool VerifySignature(byte[] publicKeyBytes)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Data);

                    // Верификация подписи
                    var publicKey = new PubKey(publicKeyBytes);
                    var ecdsaSignature = new ECDSASignature(SignatureKey);
                    bool verify = publicKey.Verify(new uint256(hash), ecdsaSignature);
                    Verify = verify;
                    return verify;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying signature: {ex.Message}");
                return false;
            }
        }

        public string ConvertSignatureToHex(byte[] signature)
        {
            string hex = BitConverter.ToString(signature).Replace("-", "").ToLower();
            SignatureHex = hex;
            return hex;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}