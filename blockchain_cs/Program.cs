using System;

class Program
{
    static void Main()
    {
        string mnemonic_phrase = MnemonicGenerator.generate_phrase();
        string privateKeyHexExample = PrivateKey.generate_private_key(mnemonic_phrase);
        string publicKeyExample = PublicKey.GeneratePublicKey(privateKeyHexExample);
        string address = AddressGenerator.GenerateReadableAddress(publicKeyExample);
        Console.WriteLine(address);
    }
}