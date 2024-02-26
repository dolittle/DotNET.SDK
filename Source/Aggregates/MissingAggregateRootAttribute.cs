// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when a <see cref="AggregateRoot" /> is missing <see cref="AggregateRootAttribute" />.
/// </summary>
public class MissingAggregateRootAttribute : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingAggregateRootAttribute"/> class.
    /// </summary>
    /// <param name="aggregateRootType">The <see cref="Type" /> of the <see cref="AggregateRoot" />.</param>
    public MissingAggregateRootAttribute(Type aggregateRootType)
        : base($"Aggregate Root {aggregateRootType} is missing [AggregateRoot] attribute")
    {
    }
}
