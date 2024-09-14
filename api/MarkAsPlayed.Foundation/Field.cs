using System.Text.Json.Serialization;

namespace MarkAsPlayed.Foundation;

public class Field
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("attributes")]
    public string[] Attributes { get; set; } = default!;

    [JsonPropertyName("connections")]
    public string[] Connections { get; set; } = default!;
}
