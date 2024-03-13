// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Builder;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.Property" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeyFromEventSourceAndOccurredAttribute : Attribute, IKeySelectorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyFromPropertyAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="occurredFormat">The date time format.</param>
    public KeyFromEventSourceAndOccurredAttribute(string occurredFormat)
    {
        OccurredFormat = occurredFormat;
    }

    /// <summary>
    /// Gets the <see cref="OccurredFormat" />.
    /// </summary>
    public OccurredFormat OccurredFormat { get; }

    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelector.EventSourceAndOccured(OccurredFormat);
}
