// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Builder;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.EventSourceId" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeyFromEventSourceAttribute : Attribute, IKeySelectorAttribute
{
    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelector.EventSource;
}
