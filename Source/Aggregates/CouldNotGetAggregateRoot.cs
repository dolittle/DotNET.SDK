// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when an aggregate root cannot be retrieved.
/// </summary>
public class CouldNotGetAggregateRoot : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotGetAggregateRoot"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <param name="error">The inner error <see cref="Exception"/>.</param>
    public CouldNotGetAggregateRoot(Type type, EventSourceId eventSourceId, Exception error)
        : base($"Could not get aggregate root of type {type} with event source id {eventSourceId}", error)
    {
    }
}
