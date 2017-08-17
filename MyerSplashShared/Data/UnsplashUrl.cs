using Newtonsoft.Json;

namespace MyerSplash.Data
{
    public class UnsplashUrl
    {
        [JsonProperty("full")]
        public string Full { get; set; }

        [JsonProperty("raw")]
        public string Raw { get; set; }

        [JsonProperty("regular")]
        public string Regular { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("thumb")]
        public string Thumb { get; set; }
    }
}
