﻿using System.Text.Json;

using NetCord.JsonModels;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Channel> GetChannelAsync(Snowflake channelId, RequestProperties? properties = null)
    {
        var json = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", properties).ConfigureAwait(false))!;
        return Channel.CreateFromJson(json.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> ModifyGroupDMChannelAsync(Snowflake channelId, Action<GroupDMChannelOptions> action, RequestProperties? properties = null)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}", new Route(RouteParameter.Channels), new JsonContent(groupDMChannelOptions), properties).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> ModifyGuildChannelAsync(Snowflake channelId, Action<GuildChannelOptions> action, RequestProperties? properties = null)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}", new(RateLimits.RouteParameter.Channels), new JsonContent(guildChannelOptions), properties).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> ModifyGuildThreadAsync(Snowflake channelId, Action<GuildThreadOptions> action, RequestProperties? properties = null)
    {
        GuildThreadOptions threadOptions = new();
        action(threadOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}", new(RateLimits.RouteParameter.Channels), new JsonContent(threadOptions), properties).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> DeleteChannelAsync(Snowflake channelId, RequestProperties? properties = null)
        => Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", properties).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);

    public Task TriggerTypingStateAsync(Snowflake channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", new Route(RouteParameter.Typing), properties);

    public async Task<IDisposable> EnterTypingStateAsync(Snowflake channelId, RequestProperties? properties = null)
    {
        TypingReminder typingReminder = new(channelId, this, properties);
        await typingReminder.StartAsync().ConfigureAwait(false);
        return typingReminder;
    }

    public async Task<IReadOnlyDictionary<Snowflake, RestMessage>> GetPinnedMessagesAsync(Snowflake channelId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", new Route(RouteParameter.GetPinnedMessages), properties).ConfigureAwait(false))!.ToObject<JsonMessage[]>().ToDictionary(m => m.Id, m => new RestMessage(m, this));

    public Task PinMessageAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", new Route(RouteParameter.PinUnpinMessage), properties);

    public Task UnpinMessageAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", new Route(RouteParameter.PinUnpinMessage), properties);

    public Task GroupDMChannelAddUserAsync(Snowflake channelId, Snowflake userId, GroupDMUserAddProperties groupDMUserAddProperties, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/recipients/{userId}", new Route(RouteParameter.Recipients, channelId), new JsonContent(groupDMUserAddProperties), properties);

    public Task GroupDMChannelDeleteUserAsync(Snowflake channelId, Snowflake userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", new Route(RouteParameter.Recipients, channelId), properties);

    public async Task<GuildThread> CreateGuildThreadAsync(Snowflake channelId, Snowflake messageId, GuildThreadFromMessageProperties threadFromMessageProperties, RequestProperties? properties = null)
     => (GuildThread)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/threads", new Route(RouteParameter.CreateGuildThread), new JsonContent(threadFromMessageProperties), properties).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);

    public async Task<GuildThread> CreateGuildThreadAsync(Snowflake channelId, GuildThreadProperties threadProperties, RequestProperties? properties = null)
        => (GuildThread)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/threads", new Route(RouteParameter.CreateGuildThread), new JsonContent(threadProperties), properties).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);

    public async Task<ForumGuildThread> CreateForumGuildThreadAsync(Snowflake channelId, ForumGuildThreadProperties threadProperties, RequestProperties? properties = null)
        => new ForumGuildThread((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/threads", new Route(RouteParameter.CreateGuildThread), threadProperties.Build(), properties).ConfigureAwait(false)).ToObject<JsonChannel>(), this);

    public Task JoinGuildThreadAsync(Snowflake threadId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", properties);

    public Task AddGuildThreadUserAsync(Snowflake threadId, Snowflake userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", properties);

    public Task LeaveGuildThreadAsync(Snowflake threadId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", properties);

    public Task DeleteGuildThreadUserAsync(Snowflake threadId, Snowflake userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", properties);

    public async Task<ThreadUser> GetGuildThreadUserAsync(Snowflake threadId, Snowflake userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", new Route(RouteParameter.GetGuildThreadUser), properties).ConfigureAwait(false))!.ToObject<JsonThreadUser>(), this);

    public async Task<IReadOnlyDictionary<Snowflake, ThreadUser>> GetGuildThreadUsersAsync(Snowflake threadId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", new Route(RouteParameter.GetGuildThreadUsers), properties).ConfigureAwait(false))!.ToObject<JsonThreadUser[]>().ToDictionary(u => u.UserId, u => new ThreadUser(u, this));

    public async IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(Snowflake channelId, RequestProperties? properties = null)
    {
        var threads = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/public?limit=100", properties).ConfigureAwait(false)).ToObject<JsonRestGuildThreadPartialResult>();
        GuildThread? last = null;
        foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
            yield return last = thread;

        if (threads.HasMore)
        {
            await foreach (var t in GetPublicArchivedGuildThreadsBeforeAsync(channelId, last!.Metadata.ArchiveTimestamp, properties))
                yield return t;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsBeforeAsync(Snowflake channelId, DateTimeOffset before, RequestProperties? properties = null)
    {
        while (true)
        {
            GuildThread? last = null;
            var threads = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/public?limit=100&before={before:s}", properties).ConfigureAwait(false)).ToObject<JsonRestGuildThreadPartialResult>();
            foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
                yield return last = thread;

            if (!threads.HasMore)
                break;

            before = last!.Metadata.ArchiveTimestamp;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(Snowflake channelId, RequestProperties? properties = null)
    {
        var threads = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/private?limit=100", properties).ConfigureAwait(false)).ToObject<JsonRestGuildThreadPartialResult>();
        GuildThread? last = null;
        foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
            yield return last = thread;

        if (threads.HasMore)
        {
            await foreach (var t in GetPrivateArchivedGuildThreadsBeforeAsync(channelId, last!.Metadata.ArchiveTimestamp, properties))
                yield return t;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsBeforeAsync(Snowflake channelId, DateTimeOffset before, RequestProperties? properties = null)
    {
        while (true)
        {
            GuildThread? last = null;
            var threads = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/private?limit=100&before={before:s}", properties).ConfigureAwait(false)).ToObject<JsonRestGuildThreadPartialResult>();
            foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
                yield return last = thread;

            if (!threads.HasMore)
                break;

            before = last!.Metadata.ArchiveTimestamp;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(Snowflake channelId, RequestProperties? properties = null)
    {
        var threads = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/users/@me/threads/archived/private?limit=100", properties).ConfigureAwait(false)).ToObject<JsonRestGuildThreadPartialResult>();
        GuildThread? last = null;
        foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
            yield return last = thread;

        if (threads.HasMore)
        {
            await foreach (var t in GetJoinedPrivateArchivedGuildThreadsBeforeAsync(channelId, last!.Id, properties))
                yield return t;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsBeforeAsync(Snowflake channelId, Snowflake before, RequestProperties? properties = null)
    {
        while (true)
        {
            GuildThread? last = null;
            var threads = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/users/@me/threads/archived/private?limit=100&before={before:s}", properties).ConfigureAwait(false)).ToObject<JsonRestGuildThreadPartialResult>();
            foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
                yield return last = thread;

            if (!threads.HasMore)
                break;

            before = last!.Id;
        }
    }

    /// <summary>
    /// Sends a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <returns></returns>
    public async Task<RestMessage> SendMessageAsync(Snowflake channelId, MessageProperties message, RequestProperties? properties = null)
    {
        var json = (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages", new Route(RouteParameter.SendMessage, channelId), message.Build(), properties).ConfigureAwait(false))!;
        return new(json.ToObject<JsonMessage>(), this);
    }

    public async Task<RestMessage> CrosspostMessageAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", new Route(RouteParameter.CrosspostMessage, channelId), properties).ConfigureAwait(false))!.ToObject<JsonMessage>(), this);

    public async Task<RestMessage> ModifyMessageAsync(Snowflake channelId, Snowflake messageId, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}/messages/{messageId}", new Route(RouteParameter.ModifyMessage), obj.Build(), properties).ConfigureAwait(false))!.ToObject<JsonMessage>(), this);
    }

    public Task DeleteMessageAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", new Route(RouteParameter.DeleteMessage, channelId), properties);

    public Task DeleteMessagesAsync(Snowflake channelId, IEnumerable<Snowflake> messageIds, RequestProperties? properties = null)
    {
        var ids = new Snowflake[100];
        int c = 0;
        List<Task> tasks = new();
        foreach (var id in messageIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteMessagesAsync(channelId, ids, properties));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteMessagesAsync(channelId, ids[..c], properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        return Task.WhenAll(tasks);
    }

    public async Task DeleteMessagesAsync(Snowflake channelId, IAsyncEnumerable<Snowflake> messageIds, RequestProperties? properties = null)
    {
        var ids = new Snowflake[100];
        int c = 0;
        List<Task> tasks = new();
        await foreach (var id in messageIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteMessagesAsync(channelId, ids, properties));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteMessagesAsync(channelId, ids[..c], properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private Task BulkDeleteMessagesAsync(Snowflake channelId, Snowflake[] messageIds, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete", new Route(RouteParameter.BulkDeleteMessages), new JsonContent($"{{\"messages\":{JsonSerializer.Serialize(messageIds, ToObjectExtensions._options)}}}"), properties);

    public Task ModifyGuildChannelPermissionsAsync(Snowflake channelId, ChannelPermissionOverwrite permissionOverwrite, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", new(RateLimits.RouteParameter.ModifyDeleteGuildChannelPermissions), new JsonContent(permissionOverwrite), properties);

    public async Task<IEnumerable<RestGuildInvite>> GetGuildChannelInvitesAsync(Snowflake channelId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", properties).ConfigureAwait(false)).ToObject<JsonRestGuildInvite[]>().Select(r => new RestGuildInvite(r, this));

    public async Task<RestGuildInvite> CreateGuildChannelInviteAsync(Snowflake channelId, GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/invites", new(RateLimits.RouteParameter.CreateGuildChannelInvite), new JsonContent(guildInviteProperties), properties).ConfigureAwait(false))!.ToObject<JsonRestGuildInvite>(), this);

    public Task DeleteGuildChannelPermissionAsync(Snowflake channelId, Snowflake overwriteId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", new RateLimits.Route(RateLimits.RouteParameter.ModifyDeleteGuildChannelPermissions), properties);

    public async Task<FollowedChannel> FollowAnnouncementGuildChannelAsync(Snowflake channelId, Snowflake targetChannelId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/followers", new JsonContent(@$"{{""webhook_channel_id"":{targetChannelId}}}"), properties).ConfigureAwait(false))!.ToObject<JsonFollowedChannel>(), this);

    public async Task<RestMessage> GetMessageAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
    => new((await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", new Route(RouteParameter.GetMessage), properties).ConfigureAwait(false))!.ToObject<JsonMessage>(), this);

    public async IAsyncEnumerable<RestMessage> GetMessagesAsync(Snowflake channelId, RequestProperties? properties = null)
    {
        byte messagesCount = 0;
        RestMessage? lastMessage = null;

        foreach (var message in await GetMaxMessagesAsyncTask(channelId, properties).ConfigureAwait(false))
        {
            yield return lastMessage = message;
            messagesCount++;
        }
        if (messagesCount == 100)
        {
            await foreach (var message in GetMessagesBeforeAsync(channelId, lastMessage!, properties))
                yield return message;
        }
    }

    public async IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
    {
        byte messagesCount;
        do
        {
            messagesCount = 0;
            foreach (var message in await GetMaxMessagesBeforeAsyncTask(channelId, messageId, properties).ConfigureAwait(false))
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == 100);
    }

    public async IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
    {
        byte messagesCount;
        do
        {
            messagesCount = 0;
            foreach (var message in await GetMaxMessagesAfterAsyncTask(channelId, messageId, properties).ConfigureAwait(false))
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == 100);
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAsyncTask(Snowflake channelId, RequestProperties? properties = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAroundAsyncTask(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&around={messageId}", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesBeforeAsyncTask(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&before={messageId}", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAfterAsyncTask(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&after={messageId}", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this)).Reverse();
    }

    public Task AddMessageReactionAsync(Snowflake channelId, Snowflake messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
    => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", new Route(RouteParameter.AddRemoveMessageReaction, channelId), properties);

    public Task DeleteMessageReactionAsync(Snowflake channelId, Snowflake messageId, ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", new Route(RouteParameter.AddRemoveMessageReaction, channelId), properties);

    public async IAsyncEnumerable<User> GetMessageReactionsAsync(Snowflake channelId, Snowflake messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
    {
        var strEmoji = ReactionEmojiToString(emoji);
        byte count = 0;
        User? lastUser = null;

        foreach (var user in await GetMaxMessageReactionsAsyncTask(channelId, messageId, strEmoji, properties).ConfigureAwait(false))
        {
            yield return lastUser = user;
            count++;
        }
        if (count == 100)
        {
            await foreach (var user in GetMessageReactionsAfterAsync(channelId, messageId, strEmoji, lastUser!.Id, properties))
                yield return user;
        }
    }

    public async IAsyncEnumerable<User> GetMessageReactionsAfterAsync(Snowflake channelId, Snowflake messageId, ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? properties = null)
    {
        var strEmoji = ReactionEmojiToString(emoji);
        byte count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxMessageReactionsAfterAsyncTask(channelId, messageId, strEmoji, userId, properties).ConfigureAwait(false))
            {
                yield return user;
                userId = user.Id;
                count++;
            }
        }
        while (count == 100);
    }

    private async Task<IEnumerable<User>> GetMaxMessageReactionsAsyncTask(Snowflake channelId, Snowflake messageId, string emoji, RequestProperties? properties = null)
    => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}?limit=100", new Route(RouteParameter.GetMessageReactions), properties).ConfigureAwait(false))!.ToObject<JsonUser[]>().Select(u => new User(u, this));

    private async Task<IEnumerable<User>> GetMaxMessageReactionsAfterAsyncTask(Snowflake channelId, Snowflake messageId, string emoji, Snowflake after, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}?limit=100&after={after}", new Route(RouteParameter.GetMessageReactions), properties).ConfigureAwait(false))!.ToObject<JsonUser[]>().Select(u => new User(u, this));

    public Task DeleteAllMessageReactionsAsync(Snowflake channelId, Snowflake messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", new Route(RouteParameter.GetMessageReactions), properties);

    public Task DeleteAllMessageReactionsAsync(Snowflake channelId, Snowflake messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", new Route(RouteParameter.GetMessageReactions), properties);

    private static string ReactionEmojiToString(ReactionEmojiProperties emoji) => emoji.EmojiType == ReactionEmojiType.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id;
}
