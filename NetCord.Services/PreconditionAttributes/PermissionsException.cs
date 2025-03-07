﻿namespace NetCord.Services;

public class PermissionsException : Exception
{
    public Permissions MissingPermissions { get; }
    public PermissionsExceptionEntityType EntityType { get; }
    public PermissionsExceptionPermissionType PermissionType { get; }

    public PermissionsException(string? message, Permissions missingPermissions, PermissionsExceptionEntityType entityType, PermissionsExceptionPermissionType permissionType) : base(message)
    {
        MissingPermissions = missingPermissions;
        EntityType = entityType;
        PermissionType = permissionType;
    }
}

public enum PermissionsExceptionEntityType
{
    User,
    Bot,
}

public enum PermissionsExceptionPermissionType
{
    Guild,
    Channel,
}
