using Windows.Data.Json;

namespace MyerSplash.Model
{
    public interface IParseFromJson
    {
        void ParseObjectFromJsonString(string json);

        void ParseObjectFromJsonObject(JsonObject json);
    }
}
