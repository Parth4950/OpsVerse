using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpsVerse.Backend.Services
{
    public static class JsonConverterService
    {
        public static readonly JsonSerializerOptions UnityCompatibleOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };
        
        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, UnityCompatibleOptions);
        }
        
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, UnityCompatibleOptions);
        }
    }
}