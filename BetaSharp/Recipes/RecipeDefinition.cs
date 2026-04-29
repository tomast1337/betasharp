using System.Text.Json;
using System.Text.Json.Serialization;
using BetaSharp.Registries.Data;

namespace BetaSharp.Recipes;

public class RecipeDefinition : DataAsset
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "shaped";

    [JsonPropertyName("pattern")]
    public string[]? Pattern { get; set; }

    /// <summary>
    /// Maps pattern characters to ingredient strings.
    /// Legacy field names or <c>name:damage</c>, or a <see cref="ResourceLocation"/> string (e.g. <c>betasharp:stone</c>).
    /// </summary>
    [JsonPropertyName("key")]
    public Dictionary<string, string>? Key { get; set; }

    /// <summary>Shapeless ingredient strings; same resolution rules as <see cref="Key"/> values.</summary>
    [JsonPropertyName("ingredients")]
    public string[]? Ingredients { get; set; }

    /// <summary>Smelting input; legacy name, <c>name:damage</c>, or <see cref="ResourceLocation"/> string. Only when type is "smelting".</summary>
    [JsonPropertyName("input")]
    public string? Input { get; set; }

    [JsonPropertyName("result")]
    public ResultRef Result { get; set; } = new();
}

public class ResultRef
{
    /// <summary>
    /// Legacy static field name (e.g. <c>IronIngot</c>), optional <c>name:damage</c>, or a
    /// <see cref="ResourceLocation"/> string (e.g. <c>betasharp:jack_o_lantern</c>).
    /// JSON may use a string or a number (numeric item id) for backward compatibility.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonConverter(typeof(RecipeIdJsonConverter))]
    public string Id { get; set; } = "";

    [JsonPropertyName("count")]
    public int Count { get; set; } = 1;
}

internal sealed class RecipeIdJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? "",
            JsonTokenType.Number => reader.GetInt32().ToString(),
            _ => throw new JsonException($"Unexpected token {reader.TokenType} for recipe result id.")
        };

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}
