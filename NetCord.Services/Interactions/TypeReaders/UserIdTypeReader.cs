﻿using System.Globalization;
using System.Reflection;

using NetCord.Services.Utils;

namespace NetCord.Services.Interactions.TypeReaders;

public class UserIdTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Interaction.Guild;
        if (guild is not null)
        {
            var users = guild.Users;
            var span = input.Span;
            var s = span.ToString();

            // by id
            if (Snowflake.TryParse(s, out ulong id))
            {
                users.TryGetValue(id, out var user);
                return new(new UserId(id, user));
            }

            // by mention
            if (Mention.TryParseUser(span, out id))
            {
                users.TryGetValue(id, out var user);
                return new(new UserId(id, user));
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
                {
                    GuildUser? user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user is not null)
                        return new(new UserId(user.Id, user));
                }
            }
            // by name or nickname
            else
            {
                var len = input.Length;
                if (len <= 32)
                {
                    if (users.Values.TryGetSingle(len >= 2 ? u => u.Username == s || u.Nickname == s : u => u.Nickname == s, out var user))
                        return new(user);

                    if (user is not null)
                        throw new AmbiguousMatchException("Too many users found.");
                }
            }
        }
        else if (context.Interaction.Channel is DMChannel dm)
        {
            var users = dm.Users;
            var span = input.Span;
            var s = span.ToString();

            // by id
            if (Snowflake.TryParse(s, out ulong id))
            {
                users.TryGetValue(id, out var user);
                return new(new UserId(id, user));
            }

            // by mention
            if (Mention.TryParseUser(span, out id))
            {
                users.TryGetValue(id, out var user);
                return new(new UserId(id, user));
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
                {
                    User? user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user is not null)
                        return new(new UserId(user.Id, user));
                }
            }
            // by name
            else
            {
                if (input.Length is <= 32 and >= 2)
                {
                    if (users.Values.TryGetSingle(u => u.Username == s, out var user))
                        return new(user);

                    if (user is not null)
                        throw new AmbiguousMatchException("Too many users found.");
                }
            }
        }

        throw new EntityNotFoundException("The user was not found.");
    }
}
