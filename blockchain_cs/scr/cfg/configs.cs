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

        // Ваши данные для записи
        string dataToWrite = "[Global]\nType=Main";

        try
        {
            // Проверяем, существует ли папка
            if (!Directory.Exists(folderPath))
            {
                // Если папка не существует, создаем ее
                Directory.CreateDirectory(folderPath);
            }

            // Используем StreamWriter для записи данных в файл
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Записываем данные в файл
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

            // Используем StreamReader для чтения данных из файла
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Читаем все содержимое файла
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
            // Проверяем, существует ли папка
            if (!Directory.Exists(folderPath))
            {
                // Если папка не существует, создаем ее
                Directory.CreateDirectory(folderPath);
            }

            // Используем StreamWriter для записи данных в файл
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Записываем данные в файл
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
        string filePath = $"{fileName}.cfg"; // Укажите полный путь к файлу .cfg

        try
        {
            // Проверяем, существует ли файл
            if (File.Exists(filePath))
            {
                // Если файл существует, удаляем его
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
                // Проверяем, содержится ли "Address=" в строке конфига
                if (line.Contains("Address="))
                {
                    // Если да, извлекаем значение после "Address="
                    string addressValue = line.Split('=')[1].Trim();

                    if (addressValue == exeFileName)
                    {
                        return addressValue;
                    }
                }
            }
        }

        // Если ничего не найдено, возвращаем null
        return null;
    }
}