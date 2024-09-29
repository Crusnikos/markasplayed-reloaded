using System.Text.Json.Serialization;

namespace MarkAsPlayed.Foundation;

public class ArticleTypeField
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = default!;

    [JsonPropertyName("article_types")]
    public string[] ArticleTypes { get; set; } = default!;
}
