﻿using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class AutocompleteInteraction : Interaction
{
    public AutocompleteInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    public override AutocompleteInteractionData Data { get; }
}
