﻿using System.Globalization;

namespace NetCord.Rest;

public class ApplicationCommand : ClientEntity, IJsonModel<JsonModels.JsonApplicationCommand>
{
    JsonModels.JsonApplicationCommand IJsonModel<JsonModels.JsonApplicationCommand>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonApplicationCommand _jsonModel;

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Type of the command.
    /// </summary>
    public ApplicationCommandType Type => _jsonModel.Type;

    /// <summary>
    /// Id of the parent application.
    /// </summary>
    public ulong ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Name of the command (1-32 characters).
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// Translations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    public string Description => _jsonModel.Description;

    /// <summary>
    /// Translations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;

    /// <summary>
    /// Default required permissions to use the command.
    /// </summary>
    public Permissions? DefaultGuildUserPermissions => _jsonModel.DefaultGuildUserPermissions;

    /// <summary>
    /// Indicates whether the command is available in DMs with the app.
    /// </summary>
    public bool DMPermission => _jsonModel.DMPermission.GetValueOrDefault();

    /// <summary>
    /// Parameters for the command (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOption> Options { get; }

    /// <summary>
    /// Indicates whether the command is enabled by default when the app is added to a guild.
    /// </summary>
    public bool DefaultPermission => _jsonModel.DefaultPermission;

    /// <summary>
    /// Indicates whether the command is age-restricted.
    /// </summary>
    public bool Nsfw => _jsonModel.Nsfw;

    /// <summary>
    /// Autoincrementing version identifier updated during substantial record changes.
    /// </summary>
    public ulong Version => _jsonModel.Version;

    public ApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandOption(o, jsonModel.Name, jsonModel.Id)).ToArray();
    }

    public override string ToString() => $"</{Name}:{Id}>";

    #region Interactions.ApplicationCommands
    public virtual Task<ApplicationCommand> ModifyAsync(Action<ApplicationCommandOptions> action, RequestProperties? properties = null) => _client.ModifyGlobalApplicationCommandAsync(ApplicationId, Id, action, properties);
    public virtual Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGlobalApplicationCommandAsync(ApplicationId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> GetGuildPermissionsAsync(ulong guildId, RequestProperties? properties = null) => _client.GetApplicationCommandGuildPermissionsAsync(ApplicationId, guildId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> OverwriteGuildPermissionsAsync(ulong guildId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null) => _client.OverwriteApplicationCommandGuildPermissionsAsync(ApplicationId, guildId, Id, newPermissions, properties);
    #endregion
}
