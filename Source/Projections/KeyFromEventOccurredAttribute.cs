using System;
using Dolittle.SDK.Projections.Builder;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.Occurred" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeyFromEventOccurredAttribute : Attribute, IKeySelectorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyFromEventOccurredAttribute"/> class.
    /// </summary>
    /// <param name="occurredFormat">The name of the property.</param>
    public KeyFromEventOccurredAttribute(string occurredFormat) => OccurredFormat = occurredFormat;

    /// <summary>
    /// Gets the <see cref="OccurredFormat" />.
    /// </summary>
    public OccurredFormat OccurredFormat { get; }

    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelectorBuilder.KeyFromEventOccurred(OccurredFormat);
}