﻿using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class PermissionOverwriteProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allow")]
    public Permissions? Allowed { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deny")]
    public Permissions? Denied { get; set; }

    public PermissionOverwriteProperties(ulong id, PermissionOverwriteType type)
    {
        Id = id;
        Type = type;
    }
}
