﻿namespace NetCord;

public class ApplicationCommandInteractionResolvedData
{
    public IReadOnlyDictionary<DiscordId, User>? Users { get; }

    public IReadOnlyDictionary<DiscordId, GuildRole>? Roles { get; }

    public IReadOnlyDictionary<DiscordId, Channel>? Channels { get; }

    public IReadOnlyDictionary<DiscordId, RestMessage>? Messages { get; }

    public IReadOnlyDictionary<DiscordId, Attachment>? Attachments { get; }

    internal ApplicationCommandInteractionResolvedData(JsonModels.JsonApplicationCommandResolvedData jsonEntity, DiscordId? guildId, RestClient client)
    {
        if (jsonEntity.GuildUsers != null)
            Users = jsonEntity.GuildUsers.ToDictionary(u => new DiscordId(u.Key), u => (User)new GuildUser(u.Value with { User = jsonEntity.Users![u.Key] }, guildId.GetValueOrDefault(), client));
        else if (jsonEntity.Users != null)
            Users = jsonEntity.Users.ToDictionary(u => new DiscordId(u.Key), u => new User(u.Value, client));

        if (jsonEntity.Roles != null)
            Roles = jsonEntity.Roles.ToDictionary(r => new DiscordId(r.Key), r => new GuildRole(r.Value, client));
        if (jsonEntity.Channels != null)
            Channels = jsonEntity.Channels.ToDictionary(c => new DiscordId(c.Key), c => Channel.CreateFromJson(c.Value, client));
        if (jsonEntity.Messages != null)
            Messages = jsonEntity.Messages.ToDictionary(c => new DiscordId(c.Key), c => new RestMessage(c.Value, client));
        if (jsonEntity.Attachments != null)
            Attachments = jsonEntity.Attachments.ToDictionary(c => new DiscordId(c.Key), c => Attachment.CreateFromJson(c.Value));
    }
}