// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.EventOccurred" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeyFromEventOccurredAttribute : Attribute, IKeySelectorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyFromEventOccurredAttribute"/> class.
    /// </summary>
    /// <param name="occurredFormat">The date time format.</param>
    public KeyFromEventOccurredAttribute(string occurredFormat) => OccurredFormat = occurredFormat;

    /// <summary>
    /// Gets the <see cref="OccurredFormat" />.
    /// </summary>
    public OccurredFormat OccurredFormat { get; }

    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelector.Occurred(OccurredFormat);
}
