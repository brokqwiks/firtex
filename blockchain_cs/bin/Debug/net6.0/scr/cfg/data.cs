using NBitcoin.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

public class SessionNetwork
{
    public string SessionFilePath { get; set; }

    public bool SessionData()
    {
        try
        {
            string sessionId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(sessionId);
                byte[] hexBytes = sha256.ComputeHash(bytes);
                string hashSessionId = Convert.ToHexString(hexBytes).ToLower();
                SessionFilePath = $"data/session_{hashSessionId}.dat";

                using (FileStream fileStream = new FileStream(SessionFilePath, FileMode.Create));
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }


    public void SaveData(string data)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(SessionFilePath, true))
            {
                writer.WriteLine(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    public void SaveNodeData(string data, string type)
    {
        try
        {
            SaveData($"{type} {data}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public List<string> ReadData()
    {
        List<string> dataList = new List<string>();

        try
        {
            if (File.Exists(SessionFilePath))
            {
                dataList = File.ReadAllLines(SessionFilePath).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading data: {ex.Message}");
        }

        return dataList;
    }

    public Dictionary<string, string> ReadNodeData()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        try
        {
            using (StreamReader reader = new StreamReader(SessionFilePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    string[] parts = line.Split(' ');
                    if (parts.Length == 2)
                    {
                        string key = parts[0];
                        string value = parts[1];

                        data[key] = value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }

        return data;
    }

    public void ClearData()
    {
        try
        {
            if (File.Exists(SessionFilePath))
            {
                File.Delete(SessionFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing data: {ex.Message}");
        }
    }
}

public class DataNetwork
{
    public static void CreateDataFile(string name)
    {
        try
        {
            string filePath = $"data/node_{name}.dat";

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating data file: {ex.Message}");
        }
    }

    public static void AddEntriesToFile(string name, string jsonResponse)
    {
        try
        {
            string filePath = $"data/node_{name}.dat";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File '{filePath}' does not exist. Creating the file.");
                CreateDataFile(name);
            }

            JObject json = JObject.Parse(jsonResponse);

            string ipAddress = json["ipAddress"]?.ToString();
            string mainPort = json["MainPort"]?.ToString();
            string blockPort = json["BlockPort"]?.ToString();
            string lastBlockPort = json["LastBlockPort"]?.ToString();
            string allblockchainPort = json["AllBlockchainPort"]?.ToString();


            List<string> lines = new List<string>
        {
            $"Name: {ipAddress}",
            $"MainPort: {mainPort}",
            $"BlockPort: {blockPort}",
            $"LastBlockPort: {lastBlockPort}",
            $"AllBlockchainPort: {allblockchainPort}"
        };

            File.AppendAllLines(filePath, lines);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding entries to data file: {ex.Message}");
        }
    }

    public static Dictionary<string, Dictionary<string, string>> GetAllBlockPortsByIp()
    {
        Dictionary<string, Dictionary<string, string>> blockPortsByIp = new Dictionary<string, Dictionary<string, string>>();

        try
        {
            string dataFolderPath = "data";
            string[] dataFiles = Directory.GetFiles(dataFolderPath);

            foreach (string filePath in dataFiles)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                if (fileNameWithoutExtension.StartsWith("node_"))
                {
                    string ipAddress = fileNameWithoutExtension.Substring("node_".Length);

                    Dictionary<string, string> dataDictionary = ReadDataFile(fileNameWithoutExtension);

                    if (!blockPortsByIp.ContainsKey(ipAddress))
                    {
                        blockPortsByIp[ipAddress] = new Dictionary<string, string>();
                    }

                    foreach (var item in dataDictionary)
                    {
                        Console.WriteLine(item);
                        blockPortsByIp[ipAddress][item.Key] = item.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting BlockPorts by IP: {ex.Message}");
        }

        return blockPortsByIp;
    }



    public static Dictionary<string, string> ReadDataFile(string fileNameWithoutExtension)
    {
        Dictionary<string, string> dataDictionary = new Dictionary<string, string>();

        try
        {
            string path = Path.Combine("data", $"{fileNameWithoutExtension}.dat");

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');

                if (parts.Length >= 2)
                {
                    string entryName = parts[0].Trim();
                    string entryValue = parts[1].Trim();

                    dataDictionary[entryName] = entryValue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading data file '{fileNameWithoutExtension}': {ex.Message}");
        }

        return dataDictionary;
    }

    public static bool CheckDataFileExists(string name)
    {
        string filePath = $"data/node_{name}.dat";
        return File.Exists(filePath);
    }

}

