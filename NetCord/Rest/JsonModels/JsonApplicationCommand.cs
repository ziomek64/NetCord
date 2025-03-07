﻿using System.Globalization;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonApplicationCommand : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; } = ApplicationCommandType.ChatInput;

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public Permissions? DefaultGuildUserPermissions { get; set; }

    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; set; }

    [JsonPropertyName("default_permission")]
    public bool DefaultPermission { get; set; } = true;

    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    [JsonPropertyName("version")]
    public ulong Version { get; set; }
}
