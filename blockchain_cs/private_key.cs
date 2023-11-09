using System;
using System.Linq;
using System.Text;
using NBitcoin;

class PrivateKey
{
    public static string generate_private_key(string phrase)
    {
        byte[] mnemonicBytes = Encoding.UTF8.GetBytes(phrase);
        string private_key = Sha256.ComputeSHA256Hash(phrase);
        bool validate_private_key = false;

        if (ValidatePrivateKey(private_key))
        {   
            validate_private_key = true;
            return private_key;
        }
        else
        {
            while (validate_private_key == false)
            {
                return generate_private_key(phrase);
            }
            return private_key;
        }

    }

    public static bool ValidatePrivateKey(string private_key)
    {
        string maxPrivateKeyHex = "FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFE BAAEDCE6 AF48A03B BFD25E8C D0364140";
        return String.Compare(private_key, maxPrivateKeyHex, StringComparison.OrdinalIgnoreCase) <= 0;
    }
}