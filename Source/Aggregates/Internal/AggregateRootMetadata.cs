// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Provides static metadata about a specific aggregate root class 
/// </summary>
/// <typeparam name="TAggregateRoot"></typeparam>
static class AggregateRootMetadata<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    // ReSharper disable StaticMemberInGenericType
    public static Func<IServiceProvider, EventSourceId, Try<TAggregateRoot>> Construct { get; }
    public static IReadOnlyDictionary<Type, MethodInfo> MethodsPerEventType { get; }
    public static bool IsStateLess { get; }
    public static AggregateRootType? AggregateRootType { get; }

    static AggregateRootMetadata()
    {
        var aggregateRootType = typeof(TAggregateRoot);

        Construct = CreateConstruct(aggregateRootType);
        MethodsPerEventType = CreateMethodsPerEventType(aggregateRootType);
        IsStateLess = MethodsPerEventType.Count == 0;
        AggregateRootType = GetAggregateRootType();
    }

    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" />.</param>
    /// <returns>The <see cref="AggregateRootId" />.</returns>
    public static AggregateRootId GetAggregateRootId()
    {
        if (AggregateRootType == null)
        {
            throw new MissingAggregateRootAttribute(typeof(TAggregateRoot));
        }

        return AggregateRootType.Id;
    }
    
    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" />.</param>
    /// <returns>The <see cref="AggregateRootId" />.</returns>
    static AggregateRootType? GetAggregateRootType()
    {
        var aggregateRootType = typeof(TAggregateRoot);
        var aggregateRootAttribute = aggregateRootType.GetCustomAttribute<AggregateRootAttribute>();
        return aggregateRootAttribute?.Type;
    }

    static IReadOnlyDictionary<Type, MethodInfo> CreateMethodsPerEventType(Type aggregateRootType)
    {
        var dictionary = new Dictionary<Type, MethodInfo>();
        foreach (var method in aggregateRootType
            .GetTypeInfo()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name.Equals("On", StringComparison.InvariantCultureIgnoreCase)))
        {
            dictionary[method.GetParameters()[0].ParameterType] = method;
        }

        return dictionary;
    }

#pragma warning disable CS0618
    static Func<IServiceProvider, EventSourceId, Try<TAggregateRoot>> CreateConstruct(Type aggregateRootType)
    {
        Func<IServiceProvider, EventSourceId, Try<TAggregateRoot>> construct;

        if (!TryGetConstructor(aggregateRootType, out var constructor, out var e))
        {
            // Not instantiatable
            return (_, _) => e;
        }

        if (HasEventSourceIdParameter(constructor))
        {
            return (provider, eventSourceId) =>
            {
                try
                {
                    var instance = ActivatorUtilities.CreateInstance<TAggregateRoot>(provider, eventSourceId);
                    instance.EventSourceId = eventSourceId;
                    return instance;
                }
                catch (Exception ex)
                {
                    return new CouldNotCreateAggregateRootInstance(typeof(TAggregateRoot), eventSourceId, ex);
                }
            };
        }

        return (provider, eventSourceId) =>
        {
            try
            {
                var instance = ActivatorUtilities.CreateInstance<TAggregateRoot>(provider);
                instance.EventSourceId = eventSourceId;
                return instance;
            }
            catch (Exception ex)
            {
                return new CouldNotCreateAggregateRootInstance(typeof(TAggregateRoot), eventSourceId, ex);
            }
        };
    }
#pragma warning restore CS0618

    static bool TryGetConstructor(Type type, [NotNullWhen(true)] out ConstructorInfo? constructor, [NotNullWhen(false)] out Exception? ex)
    {
        constructor = default;
        if (MoreThanOnePublicConstructor(type, out ex))
        {
            return false;
        }

        constructor = type.GetConstructors().SingleOrDefault() ?? type.GetConstructor(Type.EmptyTypes)!;
        return true;
    }

    static bool MoreThanOnePublicConstructor(Type type, [NotNullWhen(true)] out Exception? ex)
    {
        ex = default;
        if (type.GetConstructors().Length <= 1)
        {
            return false;
        }

        ex = new InvalidAggregateRootConstructorSignature(type, "expected at most a single public constructor");
        return true;
    }

    static bool HasEventSourceIdParameter(ConstructorInfo constructor)
        => constructor.GetParameters().Any(_ => _.ParameterType == typeof(EventSourceId));
}
