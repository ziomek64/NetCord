﻿using NetCord.Services.Interactions;

namespace NetCord.Services;

public class InteractionRequireBotChannelPermissionsAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IInteractionContext
{
    public Permissions ChannelPermissions { get; }
    public string Format { get; }

    /// <param name="channelPermissions"></param>
    /// <param name="format">{0} - missing permissions</param>
    public InteractionRequireBotChannelPermissionsAttribute(Permissions channelPermissions, string format = "Required bot channel permissions: {0}.")
    {
        ChannelPermissions = channelPermissions;
        Format = format;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context.Interaction.AppPermissions.HasValue)
        {
            var permissions = context.Interaction.AppPermissions.GetValueOrDefault();
            if (!permissions.HasFlag(ChannelPermissions))
            {
                var missingPermissions = ChannelPermissions & ~permissions;
                throw new PermissionsException(string.Format(Format, missingPermissions), missingPermissions, PermissionsExceptionEntityType.User, PermissionsExceptionPermissionType.Channel);
            }
        }
        return default;
    }
}
