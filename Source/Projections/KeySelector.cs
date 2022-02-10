// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents a projection event key selector.
/// </summary>
public class KeySelector
{
    KeySelector(KeySelectorType type, KeySelectorExpression expression, Key staticKey, OccurredFormat occurredFormat)
    {
        Type = type;
        Expression = expression;
        StaticKey = staticKey;
        OccurredFormat = occurredFormat;
    }

    /// <summary>
    /// Creates a <see cref="KeySelectorType.PartitionId"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Partition() => new(KeySelectorType.PartitionId, "", "", "");
    
    /// <summary>
    /// Creates a <see cref="KeySelectorType.EventSourceId"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector EventSource() => new(KeySelectorType.EventSourceId, "", "", "");
    
    /// <summary>
    /// Creates a <see cref="KeySelectorType.Property"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="expression">The <see cref="KeySelectorExpression"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Property(KeySelectorExpression expression) => new(KeySelectorType.Property, expression, "", "");
    
    /// <summary>
    /// Creates a <see cref="KeySelectorType.Property"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="key">The static <see cref="Key"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Static(Key key) => new(KeySelectorType.Static, "", key, "");
    
    /// <summary>
    /// Creates a <see cref="KeySelectorType.Property"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="occurredFormat">The <see cref="OccurredFormat"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Occurred(OccurredFormat occurredFormat) => new(KeySelectorType.Occurred, "", "", occurredFormat);

    /// <summary>
    /// Gets the <see cref="KeySelectorType" />.
    /// </summary>
    public KeySelectorType Type { get; }

    /// <summary>
    /// Gets the <see cref="KeySelectorExpression" />.
    /// </summary>
    public KeySelectorExpression Expression { get; }
    
    /// <summary>
    /// Gets the static <see cref="Key" />.
    /// </summary>
    public Key StaticKey { get; }
    
    /// <summary>
    /// Gets the <see cref="OccurredFormat" />.
    /// </summary>
    public OccurredFormat OccurredFormat { get; }
}
