﻿using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class SingleTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Double;

    public override ValueTask<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(float.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => Discord.ApplicationCommandOptionMaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => Discord.ApplicationCommandOptionMinValue;
}
