// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when an <see cref="AggregateRoot"/> does not follow the convention for expected
/// signature for the constructor.
/// </summary>
public class InvalidAggregateRootConstructorSignature : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAggregateRootConstructorSignature"/> class.
    /// </summary>
    /// <param name="aggregateRootType">Type of the <see cref="AggregateRoot"/> that is faulty.</param>
    /// <param name="expectations">The expectations.</param>
    public InvalidAggregateRootConstructorSignature(Type aggregateRootType, string expectations)
        : base($"Wrong constructor for aggregate root of type {aggregateRootType} - {expectations}")
    {
    }
}