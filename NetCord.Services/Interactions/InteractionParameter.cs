﻿using System.Reflection;

using NetCord.Services.Helpers;
using NetCord.Services.Utils;

namespace NetCord.Services.Interactions;

public class InteractionParameter<TContext> where TContext : IInteractionContext
{
    public InteractionTypeReader<TContext> TypeReader { get; }
    public Type ElementType { get; }
    public Type NonNullableElementType { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public bool Params { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    internal InteractionParameter(ParameterInfo parameter, MethodInfo method, InteractionServiceConfiguration<TContext> configuration)
    {
        HasDefaultValue = parameter.HasDefaultValue;

        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedDictionary(a => a.GetType());

        Type? typeReaderType;
        if (Attributes.TryGetValue(typeof(InteractionParameterAttribute), out var attributes))
        {
            var commandParameterAttribute = (InteractionParameterAttribute)attributes[0];
            typeReaderType = commandParameterAttribute.TypeReaderType;
        }
        else
            typeReaderType = null;

        var type = Type = parameter.ParameterType;
        Type elementType;
        if (Attributes.ContainsKey(typeof(ParamArrayAttribute)))
        {
            Params = true;
            elementType = ElementType = type.GetElementType()!;
        }
        else
            elementType = ElementType = type;

        (TypeReader, NonNullableElementType, DefaultValue) = ParametersHelper.GetParameterInfo<TContext, IInteractionTypeReader, InteractionTypeReader<TContext>>(elementType, parameter, typeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

        Preconditions = PreconditionsHelper.GetParameterPreconditions<TContext>(attributesIEnumerable, method);
    }

    internal async ValueTask EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        var preconditions = Preconditions;
        var count = preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            var preconditionAttribute = preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
        }
    }
}
