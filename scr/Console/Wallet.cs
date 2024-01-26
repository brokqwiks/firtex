using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WalletApp
{
    private string MainApp = "Firtex.exe";
    private string WalletDirectory = "wallets";
    public string Address { get; set; }

    public void CreateWalletApp()
    {
        if(Address != null)
        {
            if (!Directory.Exists(WalletDirectory))
            {   
                Directory.CreateDirectory(WalletDirectory);
            }
            File.Copy(MainApp, $"{Address}.exe", true);
        }
    }

    public void OpenWalletApp()
    {
        if (Address != null) { 
            string file = $"{Address}.exe";
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
                Console.WriteLine("Error");
            }
        }
    }

    public static string GetWalletAddress()
    {
        Process currentProcess = Process.GetCurrentProcess();

        string exeFilePath = currentProcess.MainModule.FileName;

        string exeFileName = Path.GetFileNameWithoutExtension(exeFilePath);

        if (exeFileName != null)
        {
            return exeFileName;
        }
        else { return null; }
    }
}