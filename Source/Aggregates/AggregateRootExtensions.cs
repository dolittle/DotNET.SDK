﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public static bool TryGetOnMethod(this AggregateRoot aggregateRoot, object @event, out MethodInfo method)
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
    /// Gets all the <see cref="IEnumerable{T}"/> of <see cref="EventType"/> that the 
    /// </summary>
    /// <param name="aggregateRoot"></param>
    /// <param name="eventTypes"></param>
    /// <returns></returns>
    public static IEnumerable<EventType> GetEventTypes(this AggregateRoot aggregateRoot, IEventTypes eventTypes)
        => GetHandleMethodsFor(aggregateRoot.GetType()).Keys.Select(eventTypes.GetFor);

    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" />.</param>
    /// <returns>The <see cref="AggregateRootId" />.</returns>
    public static AggregateRootId GetAggregateRootId(this AggregateRoot aggregateRoot)
    {
        var aggregateRootType = aggregateRoot.GetType();
        var aggregateRootAttribute = aggregateRootType.GetCustomAttribute<AggregateRootAttribute>();
        if (aggregateRootAttribute == null)
        {
            throw new MissingAggregateRootAttribute(aggregateRootType);
        }
        return aggregateRootAttribute.Type.Id;
    }

    static Dictionary<Type, MethodInfo> GetHandleMethodsFor(Type aggregateRootType)
        => typeof(AggregateRootHandleMethods<>)
            .MakeGenericType(aggregateRootType)
            .GetRuntimeField("MethodsPerEventType")
            .GetValue(null) as Dictionary<Type, MethodInfo>;

    static class AggregateRootHandleMethods<TAggregateRoot>
        where TAggregateRoot : AggregateRoot
    {
        public static readonly Dictionary<Type, MethodInfo> MethodsPerEventType = new();

        static AggregateRootHandleMethods()
        {
            var aggregateRootType = typeof(TAggregateRoot);

            foreach (var method in aggregateRootType
                         .GetTypeInfo()
                         .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                         .Where(m => m.Name.Equals("On", StringComparison.InvariantCultureIgnoreCase)))
            {
                MethodsPerEventType[method.GetParameters()[0].ParameterType] = method;
            }
        }
    }
}
