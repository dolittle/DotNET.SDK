// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents a projection event key selector.
/// </summary>
public class KeySelector
{
    KeySelector(KeySelectorType type, KeySelectorExpression expression, Key staticKey, OccurredFormat occurredFormat, Func<object, EventContext, Key>? function)
    {
        Type = type;
        Expression = expression;
        StaticKey = staticKey;
        OccurredFormat = occurredFormat;
        Function = function;
    }

    public Func<object, EventContext, Key>? Function { get; }

    /// <summary>
    /// Creates a <see cref="KeySelectorType.PartitionId"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Partition { get; } = new(KeySelectorType.PartitionId, "", "", "", null);

    /// <summary>
    /// Creates a <see cref="KeySelectorType.EventSourceId"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector EventSource { get; } = new(KeySelectorType.EventSourceId, "", "", "", null);

    public static KeySelector ByFunction<TEvent>(Func<TEvent, EventContext, Key> function) where TEvent : class
    {
        return new KeySelector(KeySelectorType.Function, "", "", "", (evt, eventContext) => function((TEvent)evt, eventContext));
    }

    /// <summary>
    /// Creates a <see cref="KeySelectorType.Property"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="expression">The <see cref="KeySelectorExpression"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Property(KeySelectorExpression expression) => new(KeySelectorType.Property, expression, "", "", null);

    /// <summary>
    /// Creates a <see cref="KeySelectorType.Property"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="key">The static <see cref="Key"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Static(Key key) => new(KeySelectorType.Static, "", key, "", null);

    /// <summary>
    /// Creates a <see cref="KeySelectorType.EventOccurred"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="occurredFormat">The <see cref="OccurredFormat"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector Occurred(OccurredFormat occurredFormat) => new(KeySelectorType.EventOccurred, "", "", occurredFormat, null);

    /// <summary>
    /// Creates a <see cref="KeySelectorType.PropertyAndEventOccurred"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="expression">The <see cref="KeySelectorExpression"/>.</param>
    /// <param name="occurredFormat">The <see cref="OccurredFormat"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector PropertyAndOccured(KeySelectorExpression expression, OccurredFormat occurredFormat) =>
        new(KeySelectorType.PropertyAndEventOccurred, expression, "", occurredFormat, null);

    /// <summary>
    /// Creates a <see cref="KeySelectorType.EventSourceIdAndOccurred"/> <see cref="KeySelector"/>.
    /// </summary>
    /// <param name="occurredFormat">The <see cref="OccurredFormat"/>.</param>
    /// <returns>The <see cref="KeySelector"/>.</returns>
    public static KeySelector EventSourceAndOccured(OccurredFormat occurredFormat) =>
        new(KeySelectorType.EventSourceIdAndOccurred, "", "", occurredFormat, null);

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

    public Key GetKey(object evt, EventContext eventContext) =>
        Type switch
        {
            KeySelectorType.PartitionId => eventContext.EventSourceId.Value,
            KeySelectorType.EventSourceId => eventContext.EventSourceId.Value,
            KeySelectorType.Static => StaticKey,
            KeySelectorType.EventOccurred => eventContext.Occurred.ToString(OccurredFormat.Value, CultureInfo.InvariantCulture),
            KeySelectorType.Property => GetProperty(evt, Expression),
            KeySelectorType.Function => Function!.Invoke(evt, eventContext),
            KeySelectorType.EventSourceIdAndOccurred =>
                $"{eventContext.EventSourceId.Value}_{eventContext.Occurred.ToString(OccurredFormat.Value, CultureInfo.InvariantCulture)}",
            KeySelectorType.PropertyAndEventOccurred =>
                $"{GetProperty(evt, Expression)}_{eventContext.Occurred.ToString(OccurredFormat.Value, CultureInfo.InvariantCulture)}",
            _ => eventContext.EventSourceId.Value
        };

    static string GetProperty(object evt, KeySelectorExpression expression)
    {
        var property = evt.GetType().GetProperty(expression) ?? throw new MissingPropertyOnEvent(evt.GetType(), expression);
        return property.GetValue(evt)?.ToString() ?? "";
    }
}

class MissingPropertyOnEvent(Type getType, KeySelectorExpression expression) : Exception($"Missing property '{expression}' on event of type '{getType}'");
