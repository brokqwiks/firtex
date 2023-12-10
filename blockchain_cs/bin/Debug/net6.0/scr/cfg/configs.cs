using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class Config
{
    public static void CreateMainConfig()
    {
        string folderPath = "cfg";
        string filePath = Path.Combine(folderPath, "main.cfg");

        string dataToWrite = "[Global]\nType=Main";

        try
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(dataToWrite);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при записи данных в файл: {ex.Message}");
        }
    }

    public static string ReadMainConfig()
    {
        try
        {
            string filePath = "cfg\\main.cfg";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not exist", filePath);
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при чтении данных из файла: {ex.Message}");
        }
    }

    public static void CreateCopyConfig(string fileName, string address)
    {
        string file = $"{fileName}.cfg";
        string folderPath = "cfg";

        string filePath = Path.Combine(folderPath, file);
        string dataToWrite = $"[Global]\nType=Copy\nData=Wallet\n[Wallet]\nAddress={address}";

        try
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(dataToWrite);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при записи данных в файл: {ex.Message}");
        }
    }

    public static void DeleteConfig(string fileName)
    {
        string filePath = $"{fileName}.cfg"; 

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
        }
    }

    public static string FindAddressInConfigs(string exeFileName)
    {
        string cfgFolderPath = "cfg";
        string[] cfgFiles = Directory.GetFiles(cfgFolderPath, "*.cfg");

        foreach (var cfgFile in cfgFiles)
        {
            string[] lines = File.ReadAllLines(cfgFile);

            foreach (var line in lines)
            {
                if (line.Contains("Address="))
                {
                    string addressValue = line.Split('=')[1].Trim();

                    if (addressValue == exeFileName)
                    {
                        return addressValue;
                    }
                }
            }
        }

        return null;
    }
}