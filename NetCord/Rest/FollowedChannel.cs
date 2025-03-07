﻿namespace NetCord.Rest;

public class FollowedChannel : ClientEntity, IJsonModel<JsonModels.JsonFollowedChannel>
{
    JsonModels.JsonFollowedChannel IJsonModel<JsonModels.JsonFollowedChannel>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonFollowedChannel _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public ulong WebhookId => _jsonModel.WebhookId;

    public FollowedChannel(JsonModels.JsonFollowedChannel jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public async Task<IGuildChannel> DeleteAsync(RequestProperties? properties = null) => (IGuildChannel)await _client.DeleteChannelAsync(Id, properties).ConfigureAwait(false);
    public IAsyncEnumerable<RestMessage> GetMessagesAsync(PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null) => _client.GetMessagesAsync(Id, paginationProperties, properties);
    public Task<IReadOnlyDictionary<ulong, RestMessage>> GetMessagesAroundAsync(ulong messageId, int? limit = null, RequestProperties? properties = null) => _client.GetMessagesAroundAsync(Id, messageId, limit, properties);
    public Task<RestMessage> GetMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.GetMessageAsync(Id, messageId, properties);
    public Task<RestMessage> SendMessageAsync(MessageProperties message, RequestProperties? properties = null) => _client.SendMessageAsync(Id, message, properties);
    public Task<RestMessage> CrosspostMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.CrosspostMessageAsync(Id, messageId, properties);
    public Task AddMessageReactionAsync(ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.AddMessageReactionAsync(Id, messageId, emoji, properties);
    public Task DeleteMessageReactionAsync(ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.DeleteMessageReactionAsync(Id, messageId, emoji, properties);
    public Task DeleteMessageReactionAsync(ulong messageId, ReactionEmojiProperties emoji, ulong userId, RequestProperties? properties = null) => _client.DeleteMessageReactionAsync(Id, messageId, emoji, userId, properties);
    public IAsyncEnumerable<User> GetMessageReactionsAsync(ulong messageId, ReactionEmojiProperties emoji, PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null) => _client.GetMessageReactionsAsync(Id, messageId, emoji, paginationProperties, properties);
    public Task DeleteAllMessageReactionsAsync(ulong messageId, RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(Id, messageId, properties);
    public Task DeleteAllMessageReactionsAsync(ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(Id, messageId, emoji, properties);
    public Task<RestMessage> ModifyMessageAsync(ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyMessageAsync(Id, messageId, action, properties);
    public Task DeleteMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.DeleteMessageAsync(Id, messageId, properties);
    public Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestProperties? properties = null) => _client.DeleteMessagesAsync(Id, messageIds, properties);
    public Task DeleteMessagesAsync(IAsyncEnumerable<ulong> messageIds, RequestProperties? properties = null) => _client.DeleteMessagesAsync(Id, messageIds, properties);
    public Task ModifyPermissionsAsync(PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    public Task<FollowedChannel> FollowAsync(ulong targetChannelId, RequestProperties? properties = null) => _client.FollowAnnouncementGuildChannelAsync(Id, targetChannelId, properties);
    public Task TriggerTypingStateAsync(RequestProperties? properties = null) => _client.TriggerTypingStateAsync(Id, properties);
    public Task<IDisposable> EnterTypingStateAsync(RequestProperties? properties = null) => _client.EnterTypingStateAsync(Id, properties);
    public Task<IReadOnlyDictionary<ulong, RestMessage>> GetPinnedMessagesAsync(RequestProperties? properties = null) => _client.GetPinnedMessagesAsync(Id, properties);
    public Task PinMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.PinMessageAsync(Id, messageId, properties);
    public Task UnpinMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.UnpinMessageAsync(Id, messageId, properties);
    public Task<GuildThread> CreateThreadAsync(ulong messageId, GuildThreadFromMessageProperties threadWithMessageProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, messageId, threadWithMessageProperties, properties);
    public Task<GuildThread> CreateThreadAsync(GuildThreadProperties threadProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, threadProperties, properties);
    public IAsyncEnumerable<GuildThread> GetPublicArchivedThreadsAsync(PaginationProperties<DateTimeOffset>? paginationProperties = null, RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsAsync(Id, paginationProperties, properties);
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedThreadsAsync(PaginationProperties<DateTimeOffset>? paginationProperties = null, RequestProperties? properties = null) => _client.GetPrivateArchivedGuildThreadsAsync(Id, paginationProperties, properties);
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedThreadsAsync(PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null) => _client.GetJoinedPrivateArchivedGuildThreadsAsync(Id, paginationProperties, properties);
    #endregion

    #region Webhook
    public Task<Webhook> CreateWebhookAsync(WebhookProperties webhookProperties, RequestProperties? properties = null) => _client.CreateWebhookAsync(Id, webhookProperties, properties);
    public Task<IReadOnlyDictionary<ulong, Webhook>> GetWebhooksAsync(ulong channelId, RequestProperties? properties = null) => _client.GetChannelWebhooksAsync(channelId, properties);
    #endregion
}
