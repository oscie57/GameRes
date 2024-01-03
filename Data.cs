using System.Text.Json.Serialization;

namespace GameRes
{
    public class RootConfig
    {
        [JsonPropertyName("default")]
        public Default Default { get; set; }
        [JsonPropertyName("options")]
        public Options Options { get; set; }
        [JsonPropertyName("data")]
        public List<Game> Data { get; set; }
    }

    public class Default
    {
        [JsonPropertyName("rotation")]
        public int Rotation { get; set; }
        [JsonPropertyName("taskbar")]
        public bool Taskbar { get; set; }
        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }
    }

    public class Options
    {
        [JsonPropertyName("info")]
        public bool Info { get; set; }
        [JsonPropertyName("network")]
        public bool Network { get; set; }
        [JsonPropertyName("version")]
        public bool Version { get; set; }
    }


    public class Game
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("info")]
        public string Info { get; set; }
        [JsonPropertyName("network")]
        public string Network { get; set; }
        [JsonPropertyName("rotation")]
        public int Rotation { get; set; }
        [JsonPropertyName("taskbar")]
        public bool Taskbar { get; set; }
        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }
    }
}
