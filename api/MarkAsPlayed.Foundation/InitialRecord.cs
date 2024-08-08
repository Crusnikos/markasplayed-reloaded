using System.Text.Json.Serialization;

namespace MarkAsPlayed.Foundation;

public class InitialRecord
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("group_sign")]
    public char GroupSign { get; set; }
}
