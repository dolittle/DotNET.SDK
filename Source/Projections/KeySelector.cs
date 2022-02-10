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

    public static KeySelector Partition() => new(KeySelectorType.PartitionId, "", "", "");
    public static KeySelector EventSource() => new(KeySelectorType.EventSourceId, "", "", "");
    public static KeySelector Property(KeySelectorExpression expression) => new(KeySelectorType.Property, expression, "", "");
    public static KeySelector Static(Key key) => new(KeySelectorType.Static, "", key, "");
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
