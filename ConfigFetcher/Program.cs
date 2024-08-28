using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

class Program
{
    static async Task Main()
    {
        // Read Azure function app's Environemnt variables list from HostedConfigs.json
        var streamReader = new StreamReader("HostedConfigs.json");
        string hostedConfigsString = await streamReader.ReadToEndAsync();
        streamReader.Close();
        streamReader.Dispose();
        // Read Azure function app's local settings from HostedConfigs.json
        streamReader = new StreamReader("Targetconfig.json");
        string targetCongfigsString = await streamReader.ReadToEndAsync();
        var jsonArray = JsonConvert.DeserializeObject<JArray>(hostedConfigsString);

        JObject result = new JObject
        {
            ["IsEncrypted"] = false,
            ["Values"] = new JObject()
        };

        foreach (var item in jsonArray)
        {
            string name = item["name"].ToString();
            string value = item["value"].ToString();
            result["Values"][name] = value;
        }

        string outputJson = result.ToString(Formatting.Indented);
        var targetConfigs = JsonConvert.DeserializeObject<ConfigsFormat>(targetCongfigsString);
        var hostedConfigs = JsonConvert.DeserializeObject<ConfigsFormat>(outputJson);

        var ouputConfigs = new ConfigsFormat();
        ouputConfigs.Values = new Dictionary<string, string>();
        foreach (var pair in hostedConfigs.Values)
        {
            if(targetConfigs.Values.ContainsKey(pair.Key))
            {
                ouputConfigs.Values.Add(pair.Key, pair.Value);
            }
        }

        outputJson = JsonConvert.SerializeObject(ouputConfigs, Formatting.Indented);
        Console.WriteLine(outputJson);
    }
}

public class ConfigsFormat
{
    public bool IsEncrypted { get; set; }
    public Dictionary<string, string> Values { get; set; }
}
