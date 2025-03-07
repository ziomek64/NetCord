﻿using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

namespace MyBot;

public class CustomCommandContext : CommandContext
{
    public GuildUser BotGuildUser => Guild!.Users[Client.ApplicationId];

    public CustomCommandContext(Message message, GatewayClient client) : base(message, client)
    {
    }
}
