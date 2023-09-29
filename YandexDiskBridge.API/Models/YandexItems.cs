using Newtonsoft.Json;

namespace YandexDiskBridge.API.Models;

public class YandexItems
{
    public class Item
    {
        [JsonProperty("file")]
        public string File { get; set; }
    }
    public class Embedded
    {
        [JsonProperty("sort")]
        public string Sort { get; set; }
        
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Response
    {
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }
        
        [JsonProperty("public_url")]
        public string PublicUrl { get; set; }
        
        [JsonProperty("_embedded")]
        public Embedded Embedded { get; set; }
    }
}