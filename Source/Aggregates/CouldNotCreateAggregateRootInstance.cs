// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when an aggregate root instance could not be created.
/// </summary>
public class CouldNotCreateAggregateRootInstance : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotCreateAggregateRootInstance"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> of the aggregate root that could not be instantiated.</param>
    public CouldNotCreateAggregateRootInstance(Type type)
        : base($"Could not create an instance of aggregate root {type}")
    {
    }
}