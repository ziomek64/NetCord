﻿namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<string> GetGatewayAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/gateway", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGateway).ConfigureAwait(false)).Url;

    public async Task<GatewayBot> GetGatewayBotAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/gateway/bot", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGatewayBot).ConfigureAwait(false));
}
