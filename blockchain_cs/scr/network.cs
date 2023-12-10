using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

public class FirtexNetwork
{
    public static bool ConnectFirtexNetwork()
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        string command = "zerotier-cli join b15644912e382a70";

        startInfo.FileName = "cmd.exe";

        startInfo.Arguments = $"/c {command}"; 

        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        if (output.Contains("200 join OK"))
        {
            process.WaitForExit();
            return true;
        }
        else
        {
            process.WaitForExit();
            return false;
        }
    }

    public static async Task<string> GetActiveNodes()
    {
        string networkId = "b15644912e382a70";
        string apiToken = "1wNLJ3xSEZb7eJtV10cHhgCfnkY0z33U";
        string apiUrl = $"https://my.zerotier.com/api/network/{networkId}/member";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return null;
        }
    }

    public static async Task<string> TestResponceNetwork()
    {
        string networkId = "b15644912e382a70";
        string apiToken = "1wNLJ3xSEZb7eJtV10cHhgCfnkY0z33U";
        string apiUrl = $"https://my.zerotier.com/api/network/{networkId}/member";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse;
                }
                else
                {
                    return null;
                }   
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public static string GetLocalIpNetwork()
    {
        try
        {
            string jsonResponce = TestResponceNetwork().Result;
            var localIPAddresses = Dns.GetHostAddresses(Dns.GetHostName())
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .Select(ip => ip.ToString())
                .ToArray();

            var devices = JArray.Parse(jsonResponce);

            foreach (var device in devices)
            {
                var deviceIPs = device["config"]["ipAssignments"].ToObject<string[]>();

                if (localIPAddresses.Intersect(deviceIPs).Any())
                {
                    return localIPAddresses.Intersect(deviceIPs).First();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return null;
    }

    public static void StartServer(string serverIp, int serverPort)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, serverPort);

        tcpListener.Start();

        while (true)
        {
            TcpClient client = tcpListener.AcceptTcpClient();
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
            clientThread.Start(client);
        }
    }

    static void HandleClient(object clientObj)
    {
        TcpClient tcpClient = (TcpClient)clientObj;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        while (true)
        {
            bytesRead = 0;

            try
            {
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
                break;

            string clientMessage = Encoding.ASCII.GetString(message, 0, bytesRead);

            if (ValidateTestJsonMessage(clientMessage).Result)
            {
                string responseMessage = "True";
                byte[] responseData = Encoding.ASCII.GetBytes(responseMessage);
                clientStream.Write(responseData, 0, responseData.Length);
            }
            else
            {
                string responseMessage = "False";
                byte[] responseData = Encoding.ASCII.GetBytes(responseMessage);
                clientStream.Write(responseData, 0, responseData.Length);
            }
        }
    }

    public static bool TestNodeServer(string ipAddress, int port)
    {
        try
        {
            using (TcpClient tcpClient = new TcpClient(ipAddress, port))
            {
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    JObject jsonMessage = new JObject();
                    jsonMessage["test"] = "test";
                    jsonMessage["ipAddress"] = GetLocalIpNetwork(); 

                    byte[] data = Encoding.ASCII.GetBytes(jsonMessage.ToString());
                    clientStream.Write(data, 0, data.Length);

                    byte[] response = new byte[4096];
                    int bytesRead = clientStream.Read(response, 0, 4096);
                    string serverResponse = Encoding.ASCII.GetString(response, 0, bytesRead);

                    tcpClient.Close();
                    if (serverResponse == "True")
                    {
                        return true;
                    }
                    else {return false;}
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> ValidateTestJsonMessage(string jsonMessage)
    {
        try
        {
            JObject jsonObject = JObject.Parse(jsonMessage);

            if (jsonObject.ContainsKey("test") && jsonObject.ContainsKey("ipAddress"))
            {

                if (jsonObject["test"].ToString() == "test")
                {
                    string ipAddress = jsonObject["ipAddress"].ToString();

                    string activeNodesJson = await GetActiveNodes();

                    bool isValidIpAddress = IsValidIpAddress(ipAddress, activeNodesJson);

                    return isValidIpAddress;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return false;
        }
    }

    public static bool IsValidIpAddress(string ipAddress, string activeNodesJson)
    {
        try
        {
            var activeNodes = JArray.Parse(activeNodesJson);

            foreach (var activeNode in activeNodes)
            {
                var nodeIPs = activeNode["config"]["ipAssignments"].ToObject<string[]>();

                if (nodeIPs.Contains(ipAddress))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return false;
        }
    }

    public static string[] ActiveIpAddressesArray(string activeNodesJson)
    {
        try
        {
            var activeNodes = JArray.Parse(activeNodesJson);

            List<string> ipAddresses = new List<string>();

            foreach (var activeNode in activeNodes)
            {
                var nodeIPs = activeNode["config"]["ipAssignments"].ToObject<string[]>();

                ipAddresses.AddRange(nodeIPs.Where(ip => ip.StartsWith("192.168.192")));
            }

            return ipAddresses.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return null;
        }
    }

    public static string ConnectionActiveAddresses(string[] activeAddressesArray)
    {
        foreach (var Address in activeAddressesArray)
        {
            bool Connect = TestNodeServer(Address, 9994);
            if (Connect)
            {
                return Address.ToString();
            }
        }
        return null;
    }
}
