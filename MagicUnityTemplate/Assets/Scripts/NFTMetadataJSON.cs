using Newtonsoft.Json;

[JsonObject]
public class NFTMetadataJSON
{
    [JsonProperty]
    public string name { get; set; }
    
    [JsonProperty]
    public string description { get; set; }

    [JsonProperty]
    public string image { get; set; }

    [JsonProperty]
    public string animation_url { get; set; }
}
