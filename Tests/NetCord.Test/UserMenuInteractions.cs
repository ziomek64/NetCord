﻿using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class UserMenuInteractions : InteractionModule<UserMenuInteractionContext>
{
    [Interaction("users")]
    public Task UsersAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedUsers)}"));
    }
}
