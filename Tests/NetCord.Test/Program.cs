﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.Interactions;

namespace NetCord.Test;

internal static class Program
{
    private static readonly GatewayClient _client = new(new(TokenType.Bot, Environment.GetEnvironmentVariable("token")!), new()
    {
        Intents = GatewayIntents.All,
        ConnectionProperties = ConnectionPropertiesProperties.IOS,
    });

    private static readonly CommandService<CommandContext> _commandService = new();
    private static readonly InteractionService<ButtonInteractionContext> _buttonInteractionService = new();
    private static readonly InteractionService<StringMenuInteractionContext> _stringMenuInteractionService = new();
    private static readonly InteractionService<UserMenuInteractionContext> _userMenuInteractionService = new();
    private static readonly InteractionService<RoleMenuInteractionContext> _roleMenuInteractionService = new();
    private static readonly InteractionService<MentionableMenuInteractionContext> _mentionableMenuInteractionService = new();
    private static readonly InteractionService<ChannelMenuInteractionContext> _channelMenuInteractionService = new();
    private static readonly InteractionService<ModalSubmitInteractionContext> _modalSubmitInteractionService = new();
    private static readonly ApplicationCommandService<SlashCommandContext, AutocompleteInteractionContext> _slashCommandService;
    private static readonly ApplicationCommandService<MessageCommandContext> _messageCommandService = new();
    private static readonly ApplicationCommandService<UserCommandContext> _userCommandService = new();

    private static readonly ServiceProvider _serviceProvider;

    static Program()
    {
        var configuration = ApplicationCommandServiceConfiguration<SlashCommandContext>.Default;
        configuration = configuration with
        {
            TypeReaders = configuration.TypeReaders.Add(typeof(Permissions), new SlashCommands.PermissionsTypeReader()),
        };
        _slashCommandService = new(configuration);

        ServiceCollection services = new();
        services.AddSingleton("wzium");
        services.AddSingleton(new HttpClient());
        services.AddSingleton(new Dictionary<ulong, SemaphoreSlim>());
        _serviceProvider = services.BuildServiceProvider();
    }

    private static async Task Main()
    {
        _client.Log += Client_Log;
        _client.MessageCreate += Client_MessageCreate;
        _client.InteractionCreate += Client_InteractionCreate;
        _client.GuildAuditLogEntryCreate += Client_GuildAuditLogEntryCreate;
        var assembly = Assembly.GetEntryAssembly()!;
        _commandService.AddCommand(["pol"], ([Optional] object? o, CommandContext context) => "xd");
        _commandService.AddModules(assembly);

        _buttonInteractionService.AddModules(assembly);
        _buttonInteractionService.AddInteraction("wziummm", (ButtonInteractionContext context) => "wzium");
        _stringMenuInteractionService.AddModules(assembly);
        _userMenuInteractionService.AddModules(assembly);
        _roleMenuInteractionService.AddModules(assembly);
        _mentionableMenuInteractionService.AddModules(assembly);
        _channelMenuInteractionService.AddModules(assembly);
        _modalSubmitInteractionService.AddModules(assembly);
        _slashCommandService.AddSlashCommand("ping", "Ping!", (SlashCommandContext context, string s) => s);
        _slashCommandService.AddModules(assembly);
        _messageCommandService.AddModules(assembly);

        _messageCommandService.AddMessageCommand("wziummm", InteractionMessageProperties (MessageCommandContext context) => new() { Components = [new ActionRowProperties([new ActionButtonProperties("wziummm", "WZIUM", ButtonStyle.Success)])] });

        _userCommandService.AddModules(assembly);

        _userCommandService.AddUserCommand("wziummm", (UserCommandContext context) => "wzium");
        ApplicationCommandServiceManager manager = new();
        manager.AddService(_slashCommandService);
        manager.AddService(_messageCommandService);
        manager.AddService(_userCommandService);

        await _client.StartAsync();
        await _client.ReadyAsync;
        try
        {
            await manager.CreateCommandsAsync(_client.Rest, _client.ApplicationId, true);
        }
        catch (RestException ex)
        {
            var error = ex.Error;
            Console.WriteLine(error is null ? "No error returned." : JsonSerializer.Serialize(error, Discord.SerializerOptions));
        }
        await Task.Delay(-1);
    }

    private static async ValueTask Client_GuildAuditLogEntryCreate(AuditLogEntry entry)
    {
        if (entry.ActionType is AuditLogEvent.ChannelUpdate)
        {
            if (entry.TryGetChange<JsonChannel, string>(c => c.Name, out var change))
                await _client.Rest.SendMessageAsync(entry.TargetId!.Value, $"old: {change.OldValue} new: {change.NewValue}");
            else
                await _client.Rest.SendMessageAsync(entry.TargetId!.Value, "Name hasn't changed");
        }
    }

    private static async ValueTask Client_InteractionCreate(Interaction interaction)
    {
        try
        {
            await (interaction switch
            {
                SlashCommandInteraction slashCommandInteraction => _slashCommandService.ExecuteAsync(new(slashCommandInteraction, _client), _serviceProvider),
                MessageCommandInteraction messageCommandInteraction => _messageCommandService.ExecuteAsync(new(messageCommandInteraction, _client), _serviceProvider),
                UserCommandInteraction userCommandInteraction => _userCommandService.ExecuteAsync(new(userCommandInteraction, _client), _serviceProvider),
                StringMenuInteraction stringMenuInteraction => _stringMenuInteractionService.ExecuteAsync(new(stringMenuInteraction, _client), _serviceProvider),
                UserMenuInteraction userMenuInteraction => _userMenuInteractionService.ExecuteAsync(new(userMenuInteraction, _client), _serviceProvider),
                RoleMenuInteraction roleMenuInteraction => _roleMenuInteractionService.ExecuteAsync(new(roleMenuInteraction, _client), _serviceProvider),
                MentionableMenuInteraction mentionableMenuInteraction => _mentionableMenuInteractionService.ExecuteAsync(new(mentionableMenuInteraction, _client), _serviceProvider),
                ChannelMenuInteraction channelMenuInteraction => _channelMenuInteractionService.ExecuteAsync(new(channelMenuInteraction, _client), _serviceProvider),
                ButtonInteraction buttonInteraction => _buttonInteractionService.ExecuteAsync(new(buttonInteraction, _client), _serviceProvider),
                AutocompleteInteraction autocompleteInteraction => _slashCommandService.ExecuteAutocompleteAsync(new(autocompleteInteraction, _client), _serviceProvider),
                ModalSubmitInteraction modalSubmitInteraction => _modalSubmitInteractionService.ExecuteAsync(new(modalSubmitInteraction, _client), _serviceProvider),
                _ => throw new("Invalid interaction."),
            });
        }
        catch (Exception ex)
        {
            InteractionMessageProperties message = new()
            {
                Content = ex.Message,
                Flags = MessageFlags.Ephemeral,
            };
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(message));
            }
            catch
            {
                try
                {
                    await interaction.SendFollowupMessageAsync(message);
                }
                catch
                {
                }
            }
        }
    }

    private static async ValueTask Client_MessageCreate(Message message)
    {
        if (!message.Author.IsBot)
        {
            const string prefix = "!";
            if (message.Content.StartsWith(prefix))
            {
                try
                {
                    await _commandService.ExecuteAsync(prefix.Length, new(message, _client), _serviceProvider);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await message.ReplyAsync(new()
                        {
                            Content = ex.Message,
                            FailIfNotExists = false,
                        });
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    private static ValueTask Client_Log(LogMessage message)
    {
        Console.ForegroundColor = message.Severity == LogSeverity.Info ? ConsoleColor.Cyan : ConsoleColor.DarkRed;
        Console.WriteLine(message);
        Console.ResetColor();
        return default;
    }
}
