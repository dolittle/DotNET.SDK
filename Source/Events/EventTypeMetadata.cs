// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.SDK.Events;

public static class EventTypeMetadata<TEventType> where TEventType : class
{
    // ReSharper disable once StaticMemberInGenericType
    /// <summary>
    /// EventType metadata for the <typeparamref name="TEventType"/>.
    /// If it does not have an <see cref="EventTypeAttribute"/> it will be <c>null</c>.
    /// </summary>
    public static EventType? EventType { get; }

    static EventTypeMetadata()
    {
        var eventTypeAttribute = typeof(TEventType).GetTypeInfo().GetCustomAttribute<EventTypeAttribute>();
        EventType = eventTypeAttribute?.GetIdentifier(typeof(TEventType));
    }
}
