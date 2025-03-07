﻿using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.Interactions;

public class InteractionContext : IInteractionContext
{
    public InteractionContext(Interaction interaction)
    {
        Interaction = interaction;
    }

    public virtual Interaction Interaction { get; }
}

public class BaseButtonInteractionContext : InteractionContext
{
    public BaseButtonInteractionContext(ButtonInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override ButtonInteraction Interaction { get; }
}

public class ButtonInteractionContext : BaseButtonInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public ButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
}

public class HttpButtonInteractionContext : BaseButtonInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpButtonInteractionContext(ButtonInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
}

public class BaseStringMenuInteractionContext : InteractionContext
{
    public BaseStringMenuInteractionContext(StringMenuInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override StringMenuInteraction Interaction { get; }
}

public class StringMenuInteractionContext : BaseStringMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public StringMenuInteractionContext(StringMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}

public class HttpStringMenuInteractionContext : BaseStringMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpStringMenuInteractionContext(StringMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}

public class BaseEntityMenuInteractionContext : InteractionContext
{
    public BaseEntityMenuInteractionContext(EntityMenuInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override EntityMenuInteraction Interaction { get; }
}

public class EntityMenuInteractionContext : BaseEntityMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public EntityMenuInteractionContext(EntityMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

public class HttpEntityMenuInteractionContext : BaseEntityMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpEntityMenuInteractionContext(EntityMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

public class BaseUserMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public BaseUserMenuInteractionContext(UserMenuInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override UserMenuInteraction Interaction { get; }
}

public class UserMenuInteractionContext : BaseUserMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public UserMenuInteractionContext(UserMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedUsers = Array.Empty<User>();
        else
        {
            var users = resolvedData.Users!;
            SelectedUsers = data.SelectedValues.Select(v => users[v]).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<User> SelectedUsers { get; }
}

public class HttpUserMenuInteractionContext : BaseUserMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpUserMenuInteractionContext(UserMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedUsers = Array.Empty<User>();
        else
        {
            var users = resolvedData.Users!;
            SelectedUsers = data.SelectedValues.Select(v => users[v]).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<User> SelectedUsers { get; }
}

public class BaseRoleMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public BaseRoleMenuInteractionContext(RoleMenuInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override RoleMenuInteraction Interaction { get; }
}

public class RoleMenuInteractionContext : BaseRoleMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public RoleMenuInteractionContext(RoleMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedRoles = Array.Empty<Role>();
        else
        {
            var roles = resolvedData.Roles!;
            SelectedRoles = data.SelectedValues.Select(v => roles[v]).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Role> SelectedRoles { get; }
}

public class HttpRoleMenuInteractionContext : BaseRoleMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpRoleMenuInteractionContext(RoleMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedRoles = Array.Empty<Role>();
        else
        {
            var roles = resolvedData.Roles!;
            SelectedRoles = data.SelectedValues.Select(v => roles[v]).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Role> SelectedRoles { get; }
}

public class BaseMentionableMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public BaseMentionableMenuInteractionContext(MentionableMenuInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override MentionableMenuInteraction Interaction { get; }
}

public class MentionableMenuInteractionContext : BaseMentionableMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public MentionableMenuInteractionContext(MentionableMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = Interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedMentionables = Array.Empty<Mentionable>();
        else
        {
            var users = resolvedData.Users;
            var roles = resolvedData.Roles;
            SelectedMentionables = users is null
                ? roles is null
                    ? Array.Empty<Mentionable>()
                    : data.SelectedValues.Select(v => new Mentionable(roles[v])).ToArray()
                : roles is null
                    ? data.SelectedValues.Select(v => new Mentionable(users[v])).ToArray()
                    : data.SelectedValues.Select(v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v])).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Mentionable> SelectedMentionables { get; }
}

public class HttpMentionableMenuInteractionContext : BaseMentionableMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpMentionableMenuInteractionContext(MentionableMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = Interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedMentionables = Array.Empty<Mentionable>();
        else
        {
            var users = resolvedData.Users;
            var roles = resolvedData.Roles;
            SelectedMentionables = users is null
                ? roles is null
                    ? Array.Empty<Mentionable>()
                    : data.SelectedValues.Select(v => new Mentionable(roles[v])).ToArray()
                : roles is null
                    ? data.SelectedValues.Select(v => new Mentionable(users[v])).ToArray()
                    : data.SelectedValues.Select(v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v])).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Mentionable> SelectedMentionables { get; }
}

public class BaseChannelMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public BaseChannelMenuInteractionContext(ChannelMenuInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override ChannelMenuInteraction Interaction { get; }
}

public class ChannelMenuInteractionContext : BaseChannelMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public ChannelMenuInteractionContext(ChannelMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedChannels = Array.Empty<Channel>();
        else
        {
            var channels = resolvedData.Channels!;
            SelectedChannels = data.SelectedValues.Select(v => channels[v]).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Channel> SelectedChannels { get; }
}

public class HttpChannelMenuInteractionContext : BaseChannelMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpChannelMenuInteractionContext(ChannelMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedChannels = Array.Empty<Channel>();
        else
        {
            var channels = resolvedData.Channels!;
            SelectedChannels = data.SelectedValues.Select(v => channels[v]).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Channel> SelectedChannels { get; }
}

public class BaseModalSubmitInteractionContext : InteractionContext
{
    public BaseModalSubmitInteractionContext(ModalSubmitInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override ModalSubmitInteraction Interaction { get; }
}

public class ModalSubmitInteractionContext : BaseModalSubmitInteractionContext, IGatewayClientContext, IUserContext, IGuildContext, IChannelContext
{
    public ModalSubmitInteractionContext(ModalSubmitInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<TextInput> Components => Interaction.Data.Components;
}

public class HttpModalSubmitInteractionContext : BaseModalSubmitInteractionContext, IRestClientContext, IUserContext, IChannelContext
{
    public HttpModalSubmitInteractionContext(ModalSubmitInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<TextInput> Components => Interaction.Data.Components;
}
