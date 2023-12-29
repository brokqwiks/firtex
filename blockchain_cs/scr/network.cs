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
    public static async Task<HandleClientResult> HandleClientMessage(string jsonMessage, Blockchain blockchain)
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
                        // Обработка сообщения типа "Test"
                        bool testResult = await ValidateTestJsonMessage(jsonMessage);
                        return new HandleClientResult { ValidationResult = testResult, ResponseMessage = testResult.ToString() };

                    case "LastBlock":   
                        // Обработка сообщения типа "LastBlock"
                        bool lastBlockResult = await ValidateLastBlockJson(jsonMessage, blockchain);
                        return new HandleClientResult { ValidationResult = lastBlockResult, ResponseMessage = lastBlockResult.ToString() };

                    case "AllBlockchain":
                        // Обработка сообщения типа "AllBlockchain"
                        string blockchainJson = await ReturnAllBLockchainResponce(blockchain);
                        var blocks = Blockchain.DeserializeBlocksFromJson(blockchainJson);
                        string senderIpAddress = jsonObject["ipAddress"]?.ToString();

                        // Отправка блоков пошагово клиенту
                        await SendBlockchainBlocks(senderIpAddress, blocks);

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

                                // Отправляем подтверждение клиенту
                                return new HandleClientResult { ValidationResult = true, ResponseMessage = "Block added successfully" };
                            }
                            else
                            {
                                // Блок уже существует
                                return new HandleClientResult { ValidationResult = true, ResponseMessage = "Block already exists" };
                            }
                        }
                        else
                        {
                            return new HandleClientResult { ValidationResult = false, ResponseMessage = "Invalid message format" };
                        }

                    // Добавьте другие варианты по мере необходимости

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



    static async Task HandleClient(object clientObj, Blockchain blockchain)
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
            Console.WriteLine(clientMessage);

            // Обработка сообщения в зависимости от его типа
            HandleClientResult result = await HandleClientMessage(clientMessage, blockchain);

            // Отправка результата клиенту
            byte[] responseData = Encoding.UTF8.GetBytes(result.ResponseMessage);
            await clientStream.WriteAsync(responseData, 0, responseData.Length);
            Console.WriteLine($"Validation result: {result.ValidationResult}");
        }
    }




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
                    // Разбираем строку и получаем IP-адрес
                    string[] parts = line.Split(' ');
                    string ipAddressWithMask = parts.LastOrDefault(part => part.Contains("/"));

                    if (!string.IsNullOrEmpty(ipAddressWithMask))
                    {
                        // Получаем адрес без маски
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

    public static void StartServer(string serverIp, int serverPort, Blockchain blockchain)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, serverPort);

        tcpListener.Start();

        while (true)
        {
            TcpClient client = tcpListener.AcceptTcpClient();
            Thread clientThread = new Thread(() => HandleClient(client, blockchain));
            clientThread.Start();
        }
    }

    public static string RemoveCrLfAndBackslashesFromJson(string jsonString)
    {
        // Используем регулярное выражение для удаления \r\n из строки JSON
        string cleanedJson = Regex.Replace(jsonString, @"\\r\\n", "");

        // Удаляем экранирование слэшей внутри строки JSON
        cleanedJson = cleanedJson.Replace("\\", "");

        return cleanedJson;
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
                    jsonMessage["type"] = "Test";
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
                    else { return false; }
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

    public static bool GetLastBlockNode(string IpAddress, Blockchain blockchain)
    {
        int port = 9994;
        try
        {
            Block LastBlock = blockchain.GetLastBlock();
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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
                    string activeNodesJson = GetActiveNodes().Result;
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

    public static string AllBlockchainNode(string IpAddress, Blockchain blockchain)
    {
        int port = 9994;
        try
        {
            using (TcpClient tcpClient = new TcpClient(IpAddress, port))
            {
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    JObject jsonMessage = new JObject();
                    jsonMessage["type"] = "AllBlockchain";

                    // Используем вашу функцию для сериализации блокчейна в JSON
                    string blockchainJson = Blockchain.SerializeBlockchainToJson(blockchain);

                    jsonMessage["ipAddress"] = GetLocalIpNetwork();
                    jsonMessage["ipAddressToSend"] = IpAddress;

                    byte[] data = Encoding.UTF8.GetBytes(jsonMessage.ToString(Formatting.None));
                    clientStream.Write(data, 0, data.Length);

                    byte[] response = new byte[4096];
                    int bytesRead = clientStream.Read(response, 0, 4096);
                    string serverResponse = Encoding.UTF8.GetString(response, 0, bytesRead);

                    tcpClient.Close();
                    return serverResponse;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public static async Task<string> ReturnAllBLockchainResponce(Blockchain localBlockchain)
    {
        string blockchainJson = Blockchain.SerializeBlockchainToJson(localBlockchain).ToString();
        return blockchainJson;
    }

    public static (bool areEqual, int differingIndex) CompareBlockchainsJson(string json1, string json2)
    {
        try
        {
            JObject obj1 = JObject.Parse(json1);
            JObject obj2 = JObject.Parse(json2);

            JArray array1 = (JArray)obj1["Blocks"];
            JArray array2 = (JArray)obj2["Blocks"];

            int minLength = Math.Min(array1.Count, array2.Count);

            for (int i = 0; i < minLength; i++)
            {
                JObject block1 = (JObject)array1[i];
                JObject block2 = (JObject)array2[i];

                if (!JToken.DeepEquals(block1, block2))
                {
                    return (false, i);
                }
            }

            // Если один из блокчейнов длиннее, возвращаем индекс первого несовпадающего блока
            if (array1.Count != array2.Count)
            {
                int differingIndex = minLength;
                return (false, differingIndex);
            }

            return (true, -1); // -1 обозначает, что все блокчейны одинаковы
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return (false, -1);
        }
    }

    public static async Task<List<int>> UpdateBlockchainFromComparison(string localBlockchainJson, string remoteBlockchainJson, Blockchain blockchain)
    {
        var comparisonResult = CompareBlockchainsJson(localBlockchainJson, remoteBlockchainJson);
        List<int> extraBlockIndexes = new List<int>();

        if (!comparisonResult.areEqual)
        {
            if (comparisonResult.differingIndex == -1)
            {
                // Блокчейны полностью различаются
                // В этом случае можно обновить весь локальный блокчейн, если необходимо
                blockchain.ClearFiles(); // Очищаем файлы локального блокчейна
                blockchain.LoadFromFile(); // Загружаем блоки из файлов
                Console.WriteLine("Local blockchain updated to match the remote one.");
            }
            else
            {
                // Блоки начиная с differingIndex отличаются
                // В этом случае можно обновить локальный блокчейн, добавив отсутствующие блоки
                for (int i = comparisonResult.differingIndex; i < remoteBlockchainJson.Length; i++)
                {
                    JObject block = (JObject)JObject.Parse(remoteBlockchainJson)["Blocks"][i];
                    string blockJson = block.ToString();

                    // Проверяем, есть ли блок в локальном блокчейне
                    if (!ContainsBlock(blockchain, blockJson))
                    {
                        // Добавляем блок в локальный блокчейн
                        blockchain.AddBlock(Blockchain.DeserializeBlock(blockJson));
                        Console.WriteLine($"Added block from the remote blockchain at index {i} to the local blockchain.");
                    }
                }

            }
        }
        else
        {
            Console.WriteLine("Local and remote blockchains are already in sync.");
        }

        return extraBlockIndexes;
    }

    public static List<int> GetExtraBlockIndexes(string localBlockchainJson, string remoteBlockchainJson)
    {
        List<int> extraBlockIndexes = new List<int>();

        try
        {
            JObject localObj = JObject.Parse(localBlockchainJson);
            JObject remoteObj = JObject.Parse(remoteBlockchainJson);

            JArray localArray = (JArray)localObj["Blocks"];
            JArray remoteArray = (JArray)remoteObj["Blocks"];

            for (int i = 0; i < localArray.Count; i++)
            {
                if (i >= remoteArray.Count || !JToken.DeepEquals(localArray[i], remoteArray[i]))
                {
                    extraBlockIndexes.Add(i);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting extra block indexes: {ex.Message}");
        }

        return extraBlockIndexes;
    }


    public static bool ContainsBlock(Blockchain blockchain, string blockJson)
    {
        JObject blockObject = JObject.Parse(blockJson);

        // Проверяем, есть ли блок в блокчейне по хэшу
        string blockHash = blockObject["BlockHash"].ToString();
        return blockchain.blocks.Any(b => b.BlockHash == blockHash);
    }

    public static async Task SendBlockToServer(string blockJson, string serverIp, int serverPort)
    {
        try
        {
            using (TcpClient client = new TcpClient(serverIp, serverPort))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Преобразование строки JSON блока в массив байт
                    byte[] requestBytes = Encoding.UTF8.GetBytes(blockJson);

                    // Отправка JSON блока на сервер
                    await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

                    Console.WriteLine("Block sent to the server.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending block to the server: {ex.Message}");
        }
    }


    public static bool ContainsBlockInJson(string json, string blockJson)
    {
        try
        {
            JObject blockObject = JObject.Parse(blockJson);

            // Проверяем, есть ли блок в JSON блокчейне по хэшу
            string blockHash = blockObject["BlockHash"].ToString();
            return json.Contains(blockHash);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing block JSON: {ex.Message}");
            return false;
        }
    }

    public static List<Block> OpenExtraBlock(List<int> extraBlocks, Blockchain blockchain)
    {   
        List<Block> extraBlockIndexes = new List<Block>();
        if(extraBlocks != null && extraBlocks.Count > 0)
        {
            foreach (var extraBlock in extraBlocks)
            {
                Block block = blockchain.blocks[extraBlock];
                extraBlockIndexes.Add(block);
            }
        }
        return extraBlockIndexes;
    }

    public static string SendExtraBlocks(string ipAddress, string jsonBlocks)
    {
        int port = 9994;
        try
        {
            using (TcpClient tcpClient = new TcpClient(ipAddress, port))
            {
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    // Создаем объект, который будет содержать информацию о запросе
                    JObject jsonMessage = new JObject();
                    jsonMessage["type"] = "AddBlock";
                    jsonMessage["blocks"] = JToken.Parse(jsonBlocks); // Преобразуем строку JSON в объект JSON

                    // Преобразуем объект JSON в строку и кодируем в байты
                    byte[] data = Encoding.UTF8.GetBytes(jsonMessage.ToString(Formatting.None));

                    // Отправляем данные в теле POST запроса
                    clientStream.Write(data, 0, data.Length);

                    // Читаем ответ от сервера
                    byte[] response = new byte[4096];
                    int bytesRead = clientStream.Read(response, 0, 4096);
                    string serverResponse = Encoding.UTF8.GetString(response, 0, bytesRead);

                    tcpClient.Close();
                    return serverResponse;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public static async Task SendBlockchainBlocks(string ipAddress, List<Block> blocks)
    {
        int port = 9994;

        try
        {
            using (TcpClient tcpClient = new TcpClient(ipAddress, port))
            {
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    foreach (var block in blocks)
                    {
                        JObject jsonMessage = new JObject();
                        jsonMessage["type"] = "AddBlock";
                        jsonMessage["block"] = Blockchain.SerializeBlockToJson(block);

                        byte[] data = Encoding.UTF8.GetBytes(jsonMessage.ToString(Formatting.None));
                        clientStream.Write(data, 0, data.Length);

                        byte[] response = new byte[4096];
                        int bytesRead = clientStream.Read(response, 0, 4096);
                        string serverResponse = Encoding.UTF8.GetString(response, 0, bytesRead);

                        Console.WriteLine(serverResponse); // Выводим ответ сервера

                        // Здесь вы можете добавить проверку успешности обработки сервером блока
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

}


