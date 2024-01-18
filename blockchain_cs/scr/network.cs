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
using Org.BouncyCastle.Utilities.Net;
using IPAddress = System.Net.IPAddress;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class HandleClientResult
{
    public bool ValidationResult { get; set; }
    public string ResponseMessage { get; set; }
}


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
                    string localIp = GetLocalIpNetwork();

                    if (!string.IsNullOrEmpty(localIp))
                    {
                        responseData = responseData.Replace(localIp, "");
                    }

                    responseData = responseData.Replace("\n", "").Replace("\r", "").Trim();

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
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string command = "zerotier-cli listnetworks";

            startInfo.FileName = "cmd.exe";

            startInfo.Arguments = $"/c {command}";

            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            process.StartInfo = startInfo;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] lines = output.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains("firtextest"))
                {
                    string[] parts = line.Split(' ');
                    string ipAddressWithMask = parts.LastOrDefault(part => part.Contains("/"));

                    if (!string.IsNullOrEmpty(ipAddressWithMask))
                    {
                        string[] ipParts = ipAddressWithMask.Split('/');
                        return ipParts[0];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return null;
    }

    public static bool GetPortsConnectionServer(string IpAddress, int port, SessionNetwork session)
    {
        try
        {
            TcpClient tcpClient = new TcpClient();

            tcpClient.ReceiveTimeout = 8000;

            Task<bool> connectTask = Task.Run(async () =>
            {
                try
                {
                    await tcpClient.ConnectAsync(IpAddress, port);
                    return true;
                }
                catch
                {
                    return false;
                }
            });

            if (connectTask.Wait(8000) && connectTask.Result)
            {
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    JObject jsonMessage = new JObject();
                    Dictionary<string, string> SessionPortsData = session.ReadNodeData();
                    if (SessionPortsData != null)
                    {
                        jsonMessage["type"] = "GetPorts";
                        jsonMessage["ipAddress"] = GetLocalIpNetwork();
                        jsonMessage["MainPort"] = SessionPortsData["Main"];
                        jsonMessage["LastBlockPort"] = SessionPortsData["LastBlock"];
                        jsonMessage["BlockPort"] = SessionPortsData["Block"];
                        jsonMessage["AllBlockchainPort"] = SessionPortsData["AllBlockchain"];

                        byte[] data = Encoding.ASCII.GetBytes(jsonMessage.ToString());
                        clientStream.Write(data, 0, data.Length);

                        byte[] response = new byte[4096];
                        int bytesRead = clientStream.Read(response, 0, 4096);
                        string serverResponse = Encoding.ASCII.GetString(response, 0, bytesRead);

                        JObject jsonResponse = JObject.Parse(serverResponse);

                        if (jsonResponse.ContainsKey("testconnection") && (bool)jsonResponse["testconnection"])
                        {
                            tcpClient.Close();
                            DataNetwork.CreateDataFile(IpAddress);
                            DataNetwork.AddEntriesToFile(IpAddress, serverResponse);
                            return true;
                        }
                        else
                        {
                            tcpClient.Close();
                            return false;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }

        return false;
    }





    public static List<string> ConnectionActiveAddresses(string[] activeAddressesArray, SessionNetwork session)
    {
        List<string> connectedAddresses = new List<string>();

        foreach (var address in activeAddressesArray)
        {
            bool connect = GetPortsConnectionServer(address, 8844, session);

            if (connect)
            {
                connectedAddresses.Add(address);
            }
        }

        return connectedAddresses;
    }


    public static bool GetLastBlockNode(string IpAddress, Blockchain blockchain, int port)
    {
        try
        {
            Block LastBlock = blockchain.GetLastBlock();
            if (LastBlock != null)
            {
                using (TcpClient tcpClient = new TcpClient(IpAddress, port))
                {
                    using (NetworkStream clientStream = tcpClient.GetStream())
                    {
                        JObject jsonMessage = new JObject();

                        jsonMessage["type"] = "LastBlock";
                        jsonMessage["LastBlock"] = LastBlock.Index.ToString();
                        jsonMessage["BlockHash"] = LastBlock.BlockHash.ToString();
                        jsonMessage["ipAddress"] = GetLocalIpNetwork();
                        jsonMessage["ipAddressToSend"] = IpAddress;
                        jsonMessage["port"] = port;

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
                        else
                        {
                            return false;
                        }

                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    
    public static async Task SendBlockchainBlocks(string ipAddress, List<Block> blocks, Blockchain blockchain, int port)
    {
        try
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.ReceiveTimeout = 100000;  
                tcpClient.SendTimeout = 100000;  

                await tcpClient.ConnectAsync(ipAddress, port);

                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    foreach (var block in blocks)
                    {
                        if (block.Index == 0) 
                        {
                            var existingGenesisBlock = blockchain.blocks[0];
                            if (existingGenesisBlock == null || !existingGenesisBlock.Equals(block))
                            {
                                JObject jsonMessage = new JObject();
                                jsonMessage["type"] = "AddBlock";
                                jsonMessage["block"] = Blockchain.SerializeBlockToJson(block);
                                jsonMessage["port"] = port;

                                byte[] data = Encoding.UTF8.GetBytes(jsonMessage.ToString(Formatting.None));
                                await clientStream.WriteAsync(data, 0, data.Length);

                                byte[] response = new byte[4096];
                                int bytesRead = await clientStream.ReadAsync(response, 0, 4096);
                                string serverResponse = Encoding.UTF8.GetString(response, 0, bytesRead);

                                Console.WriteLine(serverResponse);
                            }
                        }
                        else
                        {
                            JObject jsonMessage = new JObject();
                            jsonMessage["type"] = "AddBlock";
                            jsonMessage["block"] = Blockchain.SerializeBlockToJson(block);
                            jsonMessage["port"] = port;

                            byte[] data = Encoding.UTF8.GetBytes(jsonMessage.ToString(Formatting.None));
                            await clientStream.WriteAsync(data, 0, data.Length);

                            byte[] response = new byte[4096];
                            int bytesRead = await clientStream.ReadAsync(response, 0, 4096);
                            string serverResponse = Encoding.UTF8.GetString(response, 0, bytesRead);

                            Console.WriteLine(serverResponse);
                        }
                    }

                    JObject allBlockchainRequest = new JObject();
                    allBlockchainRequest["type"] = "AllBlockchain";

                    byte[] allBlockchainData = Encoding.UTF8.GetBytes(allBlockchainRequest.ToString(Formatting.None));
                    await clientStream.WriteAsync(allBlockchainData, 0, allBlockchainData.Length);

                    byte[] allBlockchainResponse = new byte[4096];
                    int allBlockchainBytesRead = await clientStream.ReadAsync(allBlockchainResponse, 0, 4096);
                    string allBlockchainServerResponse = Encoding.UTF8.GetString(allBlockchainResponse, 0, allBlockchainBytesRead);

                    ProcessAllBlockchainResponse(allBlockchainServerResponse, blockchain);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }



    private static void ProcessAllBlockchainResponse(string response, Blockchain blockchain)
    {
        try
        {
            List<Block> remoteBlockchain = Blockchain.DeserializeBlocksFromJson(response);

            foreach (var block in remoteBlockchain)
            {
                if (!blockchain.ContainsBlock(block))
                {
                    blockchain.AddBlock(block);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing AllBlockchain response: {ex.Message}");
        }
    }
}


