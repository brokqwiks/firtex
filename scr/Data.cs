using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataWallet
{
    private string DataFolderPath = "data";
    public string Address { set; get; }
    public string Phrase { set; get; }
    public string[] ReadData;
    public void CreateDataWallet()
    {
        try
        {
            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }
            if(Address != null && Phrase != null)
            {
                string dataName = Sha256.ComputeSHA256Hash(Address);

                using (BinaryWriter writer = new BinaryWriter(File.Open($"{DataFolderPath}/{dataName}.dat", FileMode.Create)))
                {   
                    writer.Write(Address);
                    writer.Write(Phrase);
                } 
            }
        }
        catch 
        {

        }
    }

    public void ReadWalletData()
    {
        try
        {
            if(Directory.Exists(DataFolderPath))
            {
                if (Address != null)
                {
                    string dataName = Sha256.ComputeSHA256Hash(Address);
                    using (BinaryReader reader = new BinaryReader(File.Open($"{DataFolderPath}/{dataName}.dat", FileMode.Open)))
                    {
                        string address = reader.ReadString();
                        string phrase = reader.ReadString();
                        string[] data = new string[2];
                        data[0] = address;
                        data[1] = phrase;
                        ReadData = data;
                    }
                }
            }
        }
        catch 
        {

        }
    }
}
