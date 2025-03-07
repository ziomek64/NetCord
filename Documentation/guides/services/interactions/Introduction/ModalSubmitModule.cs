﻿using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class ModalSubmitModule : InteractionModule<ModalSubmitInteractionContext>
{
    [Interaction("modal")]
    public Task ModalAsync()
    {
        return RespondAsync(InteractionCallback.Message(string.Join('\n', Context.Components.Select(c => $"{c.CustomId}: {c.Value}"))));
    }
}
