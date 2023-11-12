using System;

class Program
{
    static void Main()
    {
        string inputString = "Hello, SHA-256!";
        string sha256Hash = HashUtility.ComputeSHA256Hash(inputString);

        Console.WriteLine("Input String: " + inputString);
        Console.WriteLine("SHA-256 Hash: " + sha256Hash);
    }
}
