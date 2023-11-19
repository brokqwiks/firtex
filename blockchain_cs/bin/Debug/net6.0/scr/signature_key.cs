using System;
using NBitcoin;
using NBitcoin.Crypto;

class DigitalSignature
{
    public static byte[] SignData(byte[] data, byte[] privateKeyBytes)
    {
        var key = new Key(privateKeyBytes);
        var signature = key.Sign(new uint256(data), false);
        return signature.ToDER();
    }

    public static bool VerifySignature(byte[] data, byte[] signature, byte[] publicKeyBytes)
    {
        var publicKey = new PubKey(publicKeyBytes);
        var ecdsaSignature = new ECDSASignature(signature);
        return publicKey.Verify(new uint256(data), ecdsaSignature);
    }

    public static string ConvertSignatureToHex(byte[] signature)
    {
        return BitConverter.ToString(signature).Replace("-", "").ToLower();
    }
}
