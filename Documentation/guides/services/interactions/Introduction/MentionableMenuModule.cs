﻿using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class MentionableMenuModule : InteractionModule<MentionableMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedMentionables)}"));
    }
}
