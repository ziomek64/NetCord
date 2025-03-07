﻿using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildApplicationCommandPermissionsProperties
{
    [JsonPropertyName("id")]
    public ulong CommandId { get; set; }

    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandGuildPermissionProperties> Permissions { get; set; }

    public GuildApplicationCommandPermissionsProperties(ulong commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> permissions)
    {
        CommandId = commandId;
        Permissions = permissions;
    }
}
