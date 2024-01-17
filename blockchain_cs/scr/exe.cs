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
        if (File.Exists(file))
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = file,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                CreateNoWindow = false
            };
            Process.Start(psi);

            Thread.Sleep(600000);
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

        string exeFilePath = currentProcess.MainModule.FileName;

        string exeFileName = Path.GetFileNameWithoutExtension(exeFilePath);

        if (exeFileName != null)
        {
            return exeFileName;
        }
        else { return null; }
    }

    public static bool FindExeToAddress(string address)
    {

        string exeFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}{address}.exe";

        if (File.Exists(exeFilePath))
        {
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

            cmdProcess.StandardInput.WriteLine($"start {exeFilePath}");

            return true;
        }
        else
        {
            Console.WriteLine("Error: exe-файл не найден.");
            return false;
        }
    }

    private static void KillExistingProcess()
    {
        string exeFileName = $"{GetFileName()}.exe";
        Process[] processes = Process.GetProcessesByName(exeFileName);

        foreach (Process process in processes)
        {
            process.Kill();
            process.WaitForExit(); 
        }
    }
}