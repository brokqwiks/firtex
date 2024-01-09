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


public class ServerFirtexNetwork
{
    public static async Task<HandleClientResult> HandleClientMessage(string jsonMessage, Blockchain blockchain, SessionNetwork session)
    {
        try
        {
            JObject jsonObject = JObject.Parse(jsonMessage);

            if (jsonObject.ContainsKey("type"))
            {
                string messageType = jsonObject["type"].ToString();

                switch (messageType)
                {
                    case "Test":
                        string jsonResponse = await ValidateTestJsonMessage(jsonMessage, session);
                        if (jsonResponse != null)
                        {
                            // Возвращаем JSON с портами и testconnection = true
                            return new HandleClientResult { ValidationResult = true, ResponseMessage = jsonResponse };
                        }
                        else
                        {
                            // Возвращаем ошибку
                            return new HandleClientResult { ValidationResult = false, ResponseMessage = "Error processing test message" };
                        }

                    case "LastBlock":
                        bool lastBlockResult = await ValidateLastBlockJson(jsonMessage, blockchain);
                        return new HandleClientResult { ValidationResult = lastBlockResult, ResponseMessage = lastBlockResult.ToString() };

                    case "AllBlockchain":
                        string blockchainJson = await ReturnAllBLockchainResponce(blockchain);
                        var blocks = Blockchain.DeserializeBlocksFromJson(blockchainJson);
                        string senderIpAddress = jsonObject["ipAddress"]?.ToString();

                        await FirtexNetwork.SendBlockchainBlocks(senderIpAddress, blocks, blockchain);

                        return new HandleClientResult { ValidationResult = true, ResponseMessage = "Blocks sent successfully" };


                    case "AddBlock":
                        if (jsonObject.ContainsKey("block"))
                        {
                            string blockJson = jsonObject["block"].ToString();
                            Block block = Blockchain.DeserializeBlock(blockJson);

                            if (!blockchain.ContainsBlock(block))
                            {
                                blockchain.AddBlock(block);
                                Console.WriteLine($"Added block from the client to the local blockchain. Index: {block.Index}");

                                return new HandleClientResult { ValidationResult = true, ResponseMessage = "Block added successfully" };
                            }
                            else
                            {
                                return new HandleClientResult { ValidationResult = true, ResponseMessage = "Block already exists" };
                            }
                        }
                        else
                        {
                            return new HandleClientResult { ValidationResult = false, ResponseMessage = "Invalid message format" };
                        }


                    default:
                        Console.WriteLine($"Unknown message type: {messageType}");
                        return new HandleClientResult { ValidationResult = false, ResponseMessage = "Unknown message type." };
                }
            }
            else
            {
                Console.WriteLine("Message does not contain a 'type' field.");
                return new HandleClientResult { ValidationResult = false, ResponseMessage = "Message does not contain a 'type' field." };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new HandleClientResult { ValidationResult = false, ResponseMessage = $"Error: {ex.Message}" };
        }
    }



    static async Task HandleClient(object clientObj, Blockchain blockchain, SessionNetwork session)
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

            HandleClientResult result = await HandleClientMessage(clientMessage, blockchain, session);

            byte[] responseData = Encoding.UTF8.GetBytes(result.ResponseMessage);
            await clientStream.WriteAsync(responseData, 0, responseData.Length);
            Console.WriteLine($"Validation result: {result.ValidationResult}");
        }
    }

    public static void StartServer(string serverIp, Blockchain blockchain, SessionNetwork session, string type)
    {
        try
        {
            int port;
            Dictionary<string, string> data = session.ReadNodeData();

            if (data.Count > 0)
            {
                var lastEntry = data.Last();
                port = Int32.Parse(lastEntry.Value);
                port++;
            }
            else
            {
                port = GetAvailablePort();
            }

            session.SaveNodeData(port.ToString(), type);

            TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client, blockchain, session));
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



    public static async Task<string> ValidateTestJsonMessage(string jsonMessage, SessionNetwork session)
    {
        try
        {
            JObject jsonObject = JObject.Parse(jsonMessage);

            if (jsonObject.ContainsKey("type") && jsonObject["type"].ToString().ToLower() == "test")
            {
                Dictionary<string, string> sessionData = session.ReadNodeData();

                foreach (var entry in sessionData)
                {
                    string key = entry.Key + "Port";
                    string value = entry.Value;

                    jsonObject[key] = value;
                }

                jsonObject["testconnection"] = true;

                string jsonResponse = jsonObject.ToString();

                return jsonResponse;
            }
            else
            {
                Console.WriteLine("Invalid message type or missing 'type' field.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return null;
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

    public static async Task<bool> ValidateLastBlockJson(string jsonMessage, Blockchain blockchain)
    {
        try
        {
            Block LastBlock = blockchain.GetLastBlock();
            JObject jsonObject = JObject.Parse(jsonMessage);
            if (jsonObject != null)
            {
                if (jsonObject["BlockHash"].ToString() == LastBlock.BlockHash)
                {
                    string activeNodesJson = await FirtexNetwork.GetActiveNodes();
                    if (IsValidIpAddress(jsonObject["ipAddress"].ToString(), activeNodesJson))
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public static async Task<string> ReturnAllBLockchainResponce(Blockchain localBlockchain)
    {
        string blockchainJson = Blockchain.SerializeBlockchainToJson(localBlockchain).ToString();
        return blockchainJson;
    }

    public static int GetAvailablePort()
    { 
        int startPort = 8844;
        int endPort = 9999;

        for (int port = startPort; port <= endPort; port++)
        {
            if (IsPortAvailable(port))
            {
                return port;
            }
        }

        return -1;
    }

    public static bool IsPortAvailable(int port)
    {
        try
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
