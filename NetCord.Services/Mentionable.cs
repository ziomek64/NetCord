﻿namespace NetCord.Services;

public class Mentionable : Entity
{
    public override ulong Id { get; }
    public User? User { get; }
    public Role? Role { get; }
    public MentionableType Type { get; }

    public Mentionable(User user)
    {
        Id = user.Id;
        User = user;
        Type = MentionableType.User;
    }

    public Mentionable(Role role)
    {
        Id = role.Id;
        Role = role;
        Type = MentionableType.Role;
    }

    public override string ToString() => Type switch
    {
        MentionableType.User => User!.ToString(),
        MentionableType.Role => Role!.ToString(),
        _ => throw new ArgumentOutOfRangeException(),
    };
}

public enum MentionableType : byte
{
    User,
    Role,
}
