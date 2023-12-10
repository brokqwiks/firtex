using System;
using System.IO;

class MnemonicGenerator
{
    public static string generate_phrase()
    {
        string[] wordList = File.ReadAllLines("libs\\phrase_libs\\firtex_1010.txt");
        
        int mnemonicLength = 12;
        Random random = new Random();
        string[] mnemonicWords = new string[mnemonicLength];

        for (int i = 0; i < mnemonicLength; i++)
        {
            int randomIndex = random.Next(0, wordList.Length);
            mnemonicWords[i] = wordList[randomIndex];
        }

        string mnemonicPhrase = string.Join(" ", mnemonicWords);

        return mnemonicPhrase;
    }
}
