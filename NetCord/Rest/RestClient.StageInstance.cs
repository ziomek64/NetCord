﻿namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StageInstance> CreateStageInstanceAsync(StageInstanceProperties stageInstanceProperties, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, "/stage-instances", new(RateLimits.RouteParameter.CreateStageInstance), new JsonContent<StageInstanceProperties>(stageInstanceProperties, StageInstanceProperties.StageInstancePropertiesSerializerContext.WithOptions.StageInstanceProperties), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance).ConfigureAwait(false), this);

    public async Task<StageInstance> GetStageInstanceAsync(ulong channelId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stage-instances/{channelId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance).ConfigureAwait(false), this);

    public async Task<StageInstance> ModifyStageInstanceAsync(ulong channelId, Action<StageInstanceOptions> action, RequestProperties? properties = null)
    {
        StageInstanceOptions stageInstanceOptions = new();
        action(stageInstanceOptions);
        return new(await (await SendRequestAsync(HttpMethod.Patch, $"/stage-instances/{channelId}", new(RateLimits.RouteParameter.ModifyStageInstance), new JsonContent<StageInstanceOptions>(stageInstanceOptions, StageInstanceOptions.StageInstanceOptionsSerializerContext.WithOptions.StageInstanceOptions), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance).ConfigureAwait(false), this);
    }

    public Task DeleteStageInstanceAsync(ulong channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/stage-instances/{channelId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteStageInstance), properties);
}
