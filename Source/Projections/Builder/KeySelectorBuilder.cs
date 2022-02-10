// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents a builder for building the key selector expression for projection's on() method.
/// </summary>
public class KeySelectorBuilder
{
    /// <summary>
    /// Select projection key from the <see cref="EventSourceId"/>.
    /// </summary>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    public static KeySelector KeyFromEventSource()
        => KeySelector.EventSource();

    /// <summary>
    /// Select projection key from the <see cref="PartitionId"/>.
    /// </summary>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    public static KeySelector KeyFromPartitionId()
        => KeySelector.Partition();

    /// <summary>
    /// Select projection key from a property of the event.
    /// </summary>
    /// <param name="selectorExpression">The property on the event.</param>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    public static KeySelector KeyFromProperty(KeySelectorExpression selectorExpression)
        => KeySelector.Property(selectorExpression);
    
    /// <summary>
    /// Select projection key from a static key.
    /// </summary>
    /// <param name="staticKey">The static projection key.</param>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    public static KeySelector StaticKey(Key staticKey)
        => KeySelector.Static(staticKey);
    
    
    /// <summary>
    /// Select projection key from when an event occurred.
    /// </summary>
    /// <param name="occurredFormat">The date time format.</param>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    public static KeySelector KeyFromEventOccurred(OccurredFormat occurredFormat)
        => KeySelector.Occurred(occurredFormat);
}
