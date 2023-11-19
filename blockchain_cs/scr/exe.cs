using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public class Exe
{
    public static void CreateCopyExe(string CopyName, string address)
    {
        string sourceFilePath = "blockchain_cs.exe";
        string destinationFilePath = $"{CopyName}.exe";

        try
        {
            File.Copy(sourceFilePath, destinationFilePath, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при копировании файла: {ex.Message}");
        }

        Config.CreateCopyConfig(address, address);
    }

    public static void OpenCopyExe(string fileName)
    {
        string file = $"{fileName}.exe";
        // Проверяем существование файла
        if (File.Exists(file))
        {
            // Создаем процесс с настройками
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = file,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                CreateNoWindow = false
            };
            // Запускаем новый exe файл
            Process.Start(psi);

            Thread.Sleep(600000);
            // Завершаем текущий процесс (если нужно)
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("File not exist");
        }
    }

    public static string GetFileName()
    {
        System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

        // Получаем полный путь к запускаемому exe-файлу
        string exeFilePath = currentProcess.MainModule.FileName;

        // Извлекаем только имя файла без пути и расширения
        string exeFileName = Path.GetFileNameWithoutExtension(exeFilePath);

        if (exeFileName != null)
        {
            return exeFileName;
        }
        else { return null; }
    }

    public static bool FindExeToAddress(string address)
    {
        // Завершаем существующий процесс, если он существует

        string exeFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}{address}.exe";

        // Проверяем существует ли exe-файл
        if (File.Exists(exeFilePath))
        {
            // Найден exe-файл, запускаем его в новом окне cmd
            Console.WriteLine($"A wallet with the address {address} was found. Launch...");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            Process cmdProcess = new Process
            {
                StartInfo = psi
            };
            KillExistingProcess();
            cmdProcess.Start();

            // Передаем команду ввода в cmd
            cmdProcess.StandardInput.WriteLine($"start {exeFilePath}");

            // Завершаем функцию
            return true;
        }
        else
        {
            // Файл не найден, выводим сообщение об ошибке
            Console.WriteLine("Error: exe-файл не найден.");
            return false;
        }
    }

    private static void KillExistingProcess()
    {
        string exeFileName = $"{GetFileName()}.exe";
        // Получаем все процессы с заданным именем
        Process[] processes = Process.GetProcessesByName(exeFileName);

        // Завершаем каждый процесс
        foreach (Process process in processes)
        {
            process.Kill();
            process.WaitForExit(); // Ждем, пока процесс завершится
        }
    }
}