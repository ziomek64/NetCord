﻿using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    public partial class MessageModule
    {
        public async Task<RestMessage> GetAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonMessage>(), _client);

        public async IAsyncEnumerable<RestMessage> GetAsync(DiscordId channelId, RequestOptions? options = null)
        {
            byte messagesCount = 0;
            RestMessage? lastMessage = null;

            foreach (var message in await GetMaxMessagesAsyncTask(channelId, options).ConfigureAwait(false))
            {
                yield return lastMessage = message;
                messagesCount++;
            }
            if (messagesCount == 100)
            {
                await foreach (var message in GetBeforeAsync(channelId, lastMessage!, options))
                    yield return message;
            }
        }

        public async IAsyncEnumerable<RestMessage> GetBeforeAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
        {
            byte messagesCount;
            do
            {
                messagesCount = 0;
                foreach (var message in await GetMaxMessagesBeforeAsyncTask(channelId, messageId, options).ConfigureAwait(false))
                {
                    yield return message;
                    messageId = message.Id;
                    messagesCount++;
                }
            }
            while (messagesCount == 100);
        }

        public async IAsyncEnumerable<RestMessage> GetAfterAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
        {
            byte messagesCount;
            do
            {
                messagesCount = 0;
                foreach (var message in await GetMaxMessagesAfterAsyncTask(channelId, messageId, options).ConfigureAwait(false))
                {
                    yield return message;
                    messageId = message.Id;
                    messagesCount++;
                }
            }
            while (messagesCount == 100);
        }

        private async Task<IEnumerable<RestMessage>> GetMaxMessagesAsyncTask(DiscordId channelId, RequestOptions? options = null)
        {
            var messagsJson = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100", options).ConfigureAwait(false))!;
            return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), _client));
        }

        private async Task<IEnumerable<RestMessage>> GetMaxMessagesAroundAsyncTask(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
        {
            var messagsJson = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&around={messageId}", options).ConfigureAwait(false))!;
            return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), _client));
        }

        private async Task<IEnumerable<RestMessage>> GetMaxMessagesBeforeAsyncTask(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
        {
            var messagsJson = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&before={messageId}", options).ConfigureAwait(false))!;
            return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), _client));
        }

        private async Task<IEnumerable<RestMessage>> GetMaxMessagesAfterAsyncTask(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
        {
            var messagsJson = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&after={messageId}", options).ConfigureAwait(false))!;
            return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), _client));
        }
    }
}