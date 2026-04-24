using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDataService
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.Preserve,
        Converters = { new JsonStringEnumConverter() }
    };

    public List<T> LoadData<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<T>();

        var json = File.ReadAllText(filePath);

        return string.IsNullOrWhiteSpace(json)
            ? new List<T>()
            : JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
    }

    public void SaveData<T>(string filePath, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(filePath, json);
    }
}