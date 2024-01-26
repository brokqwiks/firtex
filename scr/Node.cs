﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using IPAddress = System.Net.IPAddress;
using System.ComponentModel;
using NBitcoin;
using System.Diagnostics;

public class HandleClientResult
{
    public bool ValidationResult { get; set; }
    public string ResponceMessage { get; set; }
}

public class FirtexNode
{
    public int[] ports;
    public Blockchain blockchain { get; set; }
    public string Data { get; set; }
    public string LocalIp;
    public string[] ActiveNodes

    public bool ConnectFirtexNetwork()
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


    private async Task<HandleClientResult> HandleClientMessage(string jsonMessage)
    {
        try 
        {
            JObject json = JObject.Parse(jsonMessage);
            if(json.ContainsKey("type") && json["type"] != null)
            {
                string messageType = json["type"].ToString();
                switch(messageType)
                {
                    case "Active":
                        return new HandleClientResult { ValidationResult = true, ResponceMessage = "200" };

                    case "AddBlock":
                        bool addblock = await AddBlockValidate(json);
                        return new HandleClientResult { ValidationResult = addblock, ResponceMessage = addblock.ToString() };

                }
            }
            return null;
        }
        catch {
            return null;
        }
    }

    private async Task<bool> AddBlockValidate(JObject json)
    {
        try
        {
            if (json.ContainsKey("block"))
            {
                string blockJson = json["block"].ToString();
                Block block = blockchain.DeserializeBlockJson(blockJson);
                if (block != null)
                {
                    byte[] blockData = Encoding.UTF8.GetBytes(block.Data);
                    byte[] publicKey = Keys.PublicKey.StringToByteArray(block.PublicKeySender);
                    byte[] signature = Keys.Signature.HexStringToByteArray(block.SignatureKey);
                    Keys.Signature Signature = new Keys.Signature();
                    Signature.SignatureKey = signature;
                    Signature.VerifySignature(publicKey);
                    if (Signature.Verify) 
                    {
                        blockchain.AddBlock(block);
                        return true;
                    }
                }
            }
            return false;
        }
        catch(Exception ex)
        {
            return false;
        }
    }

    public async Task HandleClient(object clientObj)
    {
        TcpClient tcpClient = (TcpClient)clientObj;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] messageBytes = new byte[4096];
        int bytesRead;

        while (true)
        {
            bytesRead = 0;

            try
            {
                bytesRead = await clientStream.ReadAsync(messageBytes, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
                break;

            string clientMessage = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);

            HandleClientResult result = await HandleClientMessage(clientMessage);

            byte[] responseData = Encoding.UTF8.GetBytes(result.ResponceMessage);
            await clientStream.WriteAsync(responseData, 0, responseData.Length);
            Console.WriteLine($"Validation result: {result.ValidationResult}");
        }
    }

    public void StartServer()
    {
        try
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, ports[0]);
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public string GetLocalIpNetwork()
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
                        LocalIp = ipParts[0];
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

    public async Task<string[]> GetActiveNodes()
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
                    var activeNodes = JArray.Parse(responseData);

                    List<string> ipAddresses = new List<string>();

                    foreach (var activeNode in activeNodes)
                    {
                        var nodeIPs = activeNode["config"]["ipAssignments"].ToObject<string[]>();

                        ipAddresses.AddRange(nodeIPs.Where(ip => ip.StartsWith("192.168.192")));
                    }

                    ActiveNodes = ipAddresses.ToArray();
                    return ipAddresses.ToArray();


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

    public bool ActiveConnect(string IpAddress, int port)
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
                        jsonMessage["type"] = "Active";

                        byte[] data = Encoding.ASCII.GetBytes(jsonMessage.ToString());
                        clientStream.Write(data, 0, data.Length);

                        byte[] response = new byte[4096];
                        int bytesRead = clientStream.Read(response, 0, 4096);
                        string serverResponse = Encoding.ASCII.GetString(response, 0, bytesRead);
                        if (serverResponse == "200") 
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }

        return false;
    }
}