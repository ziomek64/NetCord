﻿using System.Text;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildProperties>(guildProperties, Serialization.Default.GuildProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    public async Task<RestGuild> GetGuildAsync(ulong guildId, bool withCounts = false, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}", $"?with_counts={withCounts}", new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);

    public async Task<GuildPreview> GetGuildPreviewAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);

    public async Task<RestGuild> ModifyGuildAsync(ulong guildId, Action<GuildOptions> action, RequestProperties? properties = null)
    {
        GuildOptions guildOptions = new();
        action(guildOptions);
        using (HttpContent content = new JsonContent<GuildOptions>(guildOptions, Serialization.Default.GuildOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    public Task DeleteGuildAsync(ulong guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", null, new(guildId), properties);

    public async Task<IReadOnlyDictionary<ulong, IGuildChannel>> GetGuildChannelsAsync(ulong guildId, RequestProperties? properties = null)
    => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannelArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => (IGuildChannel)Channel.CreateFromJson(c, this));

    public async Task<IGuildChannel> CreateGuildChannelAsync(ulong guildId, GuildChannelProperties channelProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildChannelProperties>(channelProperties, Serialization.Default.GuildChannelProperties))
            return (IGuildChannel)Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/channels", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task ModifyGuildChannelPositionsAsync(ulong guildId, IEnumerable<GuildChannelPositionProperties> positions, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<GuildChannelPositionProperties>>(positions, Serialization.Default.IEnumerableGuildChannelPositionProperties))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/channels", null, new(guildId), properties).ConfigureAwait(false);
    }

    public async Task<IReadOnlyDictionary<ulong, GuildThread>> GetActiveGuildThreadsAsync(ulong guildId, RequestProperties? properties = null)
        => GuildThreadGenerator.CreateThreads(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildThreadResult).ConfigureAwait(false), this);

    public async Task<GuildUser> GetGuildUserAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);

    public IAsyncEnumerable<GuildUser> GetGuildUsersAsync(ulong guildId, PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.After, 1000);

        return new PaginationAsyncEnumerable<GuildUser, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildUserArray).ConfigureAwait(false)).Select(u => new GuildUser(u, guildId, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/members",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString()),
            new(guildId),
            properties);
    }

    public async Task<IReadOnlyDictionary<ulong, GuildUser>> FindGuildUserAsync(ulong guildId, string name, int limit, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search", $"?query={Uri.EscapeDataString(name)}&limit={limit}", new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUserArray).ConfigureAwait(false)).ToDictionary(u => u.User.Id, u => new GuildUser(u, guildId, this));

    public async Task<GuildUser?> AddGuildUserAsync(ulong guildId, ulong userId, GuildUserProperties userProperties, RequestProperties? properties = null)
    {
        Stream? stream;
        using (HttpContent content = new JsonContent<GuildUserProperties>(userProperties, Serialization.Default.GuildUserProperties))
            stream = await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties).ConfigureAwait(false);
        if (stream.Length == 0)
            return null;
        else
            return new(await stream.ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    public async Task<GuildUser> ModifyGuildUserAsync(ulong guildId, ulong userId, Action<GuildUserOptions> action, RequestProperties? properties = null)
    {
        GuildUserOptions guildUserOptions = new();
        action(guildUserOptions);
        using (HttpContent content = new JsonContent<GuildUserOptions>(guildUserOptions, Serialization.Default.GuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    public async Task<GuildUser> ModifyCurrentGuildUserAsync(ulong guildId, Action<CurrentGuildUserOptions> action, RequestProperties? properties = null)
    {
        CurrentGuildUserOptions currentGuildUserOptions = new();
        action(currentGuildUserOptions);
        using (HttpContent content = new JsonContent<CurrentGuildUserOptions>(currentGuildUserOptions, Serialization.Default.CurrentGuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/members/@me", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    public Task AddGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", null, new(guildId), properties);

    public Task RemoveGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", null, new(guildId), properties);

    public Task KickGuildUserAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties);

    public IAsyncEnumerable<GuildBan> GetGuildBansAsync(ulong guildId, PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.After, 1000);

        return new PaginationAsyncEnumerable<GuildBan, ulong>(
            this,
            paginationProperties,
            paginationProperties.Direction.GetValueOrDefault() switch
            {
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildBanArray).ConfigureAwait(false)).Select(b => new GuildBan(b, guildId, this)),
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildBanArray).ConfigureAwait(false)).GetReversedIEnumerable().Select(b => new GuildBan(b, guildId, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            b => b.User.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/bans",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString()),
            new(guildId),
            properties);
    }

    public async Task<GuildBan> GetGuildBanAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildBan).ConfigureAwait(false), guildId, this);

    public async Task BanGuildUserAsync(ulong guildId, ulong userId, int deleteMessageSeconds = 0, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildBanProperties>(new(deleteMessageSeconds), Serialization.Default.GuildBanProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties).ConfigureAwait(false);
    }

    public Task UnbanGuildUserAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties);

    public async Task<IReadOnlyDictionary<ulong, Role>> GetRolesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRoleArray).ConfigureAwait(false)).ToDictionary(r => r.Id, r => new Role(r, guildId, this));

    public async Task<Role> CreateRoleAsync(ulong guildId, RoleProperties guildRoleProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<RoleProperties>(guildRoleProperties, Serialization.Default.RoleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/roles", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);
    }

    public async Task<IReadOnlyDictionary<ulong, Role>> ModifyRolePositionsAsync(ulong guildId, IEnumerable<RolePositionProperties> positions, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<RolePositionProperties>>(positions, Serialization.Default.IEnumerableRolePositionProperties))
            return (await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/roles", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRoleArray).ConfigureAwait(false)).ToDictionary(r => r.Id, r => new Role(r, guildId, this));
    }

    public async Task<Role> ModifyRoleAsync(ulong guildId, ulong roleId, Action<RoleOptions> action, RequestProperties? properties = null)
    {
        RoleOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<RoleOptions>(obj, Serialization.Default.RoleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);
    }

    public Task DeleteRoleAsync(ulong guildId, ulong roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties);

    public async Task<MfaLevel> ModifyGuildMfaLevelAsync(ulong guildId, MfaLevel mfaLevel, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildMfaLevelProperties>(new GuildMfaLevelProperties(mfaLevel), Serialization.Default.GuildMfaLevelProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/mfa", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildMfaLevel).ConfigureAwait(false)).Level;
    }

    public async Task<int> GetGuildPruneCountAsync(ulong guildId, int days, IEnumerable<ulong>? roles = null, RequestProperties? properties = null)
    {
        var query = roles is null
            ? $"?days={days}"
            : new StringBuilder()
                .Append("?days=")
                .Append(days)
                .Append("&include_roles=")
                .AppendJoin(',', roles)
                .ToString();
        return (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune", query, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildPruneCountResult).ConfigureAwait(false)).Pruned;
    }

    public async Task<int?> GuildPruneAsync(ulong guildId, GuildPruneProperties pruneProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildPruneProperties>(pruneProperties, Serialization.Default.GuildPruneProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/prune", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildPruneResult).ConfigureAwait(false)).Pruned;
    }

    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));

    public async Task<IEnumerable<RestGuildInvite>> GetGuildInvitesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildInviteArray).ConfigureAwait(false)).Select(i => new RestGuildInvite(i, this));

    public async Task<IReadOnlyDictionary<ulong, Integration>> GetGuildIntegrationsAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonIntegrationArray).ConfigureAwait(false)).ToDictionary(i => i.Id, i => new Integration(i, this));

    public Task DeleteGuildIntegrationAsync(ulong guildId, ulong integrationId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", null, new(guildId), properties);

    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidgetSettings).ConfigureAwait(false));

    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(ulong guildId, Action<GuildWidgetSettingsOptions> action, RequestProperties? properties = null)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        using (HttpContent content = new JsonContent<GuildWidgetSettingsOptions>(guildWidgetSettingsOptions, Serialization.Default.GuildWidgetSettingsOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/widget", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidgetSettings).ConfigureAwait(false));
    }

    public async Task<GuildWidget> GetGuildWidgetAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidget).ConfigureAwait(false), this);

    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildVanityInvite).ConfigureAwait(false));

    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWelcomeScreen).ConfigureAwait(false));

    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(ulong guildId, Action<GuildWelcomeScreenOptions> action, RequestProperties? properties = null)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        using (HttpContent content = new JsonContent<GuildWelcomeScreenOptions>(guildWelcomeScreenOptions, Serialization.Default.GuildWelcomeScreenOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/welcome-screen", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWelcomeScreen).ConfigureAwait(false));
    }

    public async Task<GuildOnboarding> GetGuildOnboardingAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/onboarding", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildOnboarding).ConfigureAwait(false), this);

    public async Task<GuildOnboarding> ModifyGuildOnboardingAsync(ulong guildId, Action<GuildOnboardingOptions> action, RequestProperties? properties = null)
    {
        GuildOnboardingOptions guildOnboardingOptions = new();
        action(guildOnboardingOptions);
        using (HttpContent content = new JsonContent<GuildOnboardingOptions>(guildOnboardingOptions, Serialization.Default.GuildOnboardingOptions))
            return new(await (await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/onboarding", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildOnboarding).ConfigureAwait(false), this);
    }

    public async Task ModifyCurrentGuildUserVoiceStateAsync(ulong guildId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? properties = null)
    {
        CurrentUserVoiceStateOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<CurrentUserVoiceStateOptions>(obj, Serialization.Default.CurrentUserVoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/voice-states/@me", null, new(guildId), properties).ConfigureAwait(false);
    }

    public async Task ModifyGuildUserVoiceStateAsync(ulong guildId, ulong channelId, ulong userId, Action<VoiceStateOptions> action, RequestProperties? properties = null)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        using (HttpContent content = new JsonContent<VoiceStateOptions>(obj, Serialization.Default.VoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/voice-states/{userId}", null, new(guildId), properties).ConfigureAwait(false);
    }
}
