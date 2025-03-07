﻿using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class TextInputProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.TextInput;

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("style")]
    public TextInputStyle Style { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    public TextInputProperties(string customId, TextInputStyle style, string label)
    {
        CustomId = customId;
        Style = style;
        Label = label;
    }
}
