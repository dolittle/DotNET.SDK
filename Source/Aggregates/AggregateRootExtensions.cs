﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Extensions for <see cref="AggregateRoot"/>.
/// </summary>
public static class AggregateRootExtensions
{
    /// <summary>
    /// Try get handle method from an <see cref="AggregateRoot"/> for a specific event.
    /// </summary>
    /// <param name="aggregateRoot"><see cref="AggregateRoot"/> to get method from.</param>
    /// <param name="event">The event to get On-method for.</param>
    /// <param name="method">The outputted <see cref="MethodInfo" />.</param>
    /// <returns>A value indicating whether there was an On-method for the event.</returns>
    public static bool TryGetOnMethod(this AggregateRoot aggregateRoot, object @event, [NotNullWhen(true)] out MethodInfo? method)
    {
        var eventType = @event.GetType();
        var handleMethods = GetHandleMethodsFor(aggregateRoot.GetType());
        return handleMethods.TryGetValue(eventType, out method);
    }

    /// <summary>
    /// Indicates whether the Aggregate Root maintains state and requires handling events to restore that state.
    /// </summary>
    /// <param name="aggregateRoot"><see cref="AggregateRoot"/> to test for statelessness.</param>
    /// <returns>true if the Aggregate Root does not maintain state.</returns>
    public static bool IsStateless(this AggregateRoot aggregateRoot)
        => GetHandleMethodsFor(aggregateRoot.GetType()).Count == 0;

    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" />.</param>
    /// <returns>The <see cref="AggregateRootId" />.</returns>
    public static AggregateRootId GetAggregateRootId(this AggregateRoot aggregateRoot)
    {
        var aggregateRootType = aggregateRoot.GetType();
        if (!aggregateRootType.TryGetIdentifier<AggregateRootType>(out var identifier))
        {
            throw new MissingAggregateRootAttribute(aggregateRootType);
        }
        return identifier.Id;
    }
    
    
#pragma warning disable CS8602 // Only used internally with the correct type. 
#pragma warning disable CS8603 // Analyzers are not able to check that this is always valid
    internal static IReadOnlyDictionary<Type, MethodInfo> GetHandleMethodsFor(Type aggregateRootType)
        => typeof(AggregateRootHandleMethods<>)
            .MakeGenericType(aggregateRootType)
            .GetRuntimeField("MethodsPerEventType")
            .GetValue(null) as IReadOnlyDictionary<Type, MethodInfo>;
#pragma warning restore CS8603
#pragma warning restore CS8602

    internal static IReadOnlyDictionary<Type, MethodInfo> GetHandleMethodsFor<TAggregateRoot>() where TAggregateRoot : AggregateRoot
        => AggregateRootMetadata<TAggregateRoot>.MethodsPerEventType;

    static class AggregateRootHandleMethods<TAggregateRoot>
        where TAggregateRoot : AggregateRoot
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly IReadOnlyDictionary<Type, MethodInfo> MethodsPerEventType = AggregateRootMetadata<TAggregateRoot>.MethodsPerEventType;
    }
}
