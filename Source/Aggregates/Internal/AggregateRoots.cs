// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRoots"/>.
/// </summary>
public class AggregateRoots : IAggregateRoots
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoots"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public AggregateRoots(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Try<TAggregate> TryGet<TAggregate>(EventSourceId eventSourceId, IServiceProvider provider)
        where TAggregate : AggregateRoot
    {
        _logger.Get(typeof(TAggregate), eventSourceId);
        return AggregateRootMetadata<TAggregate>.Construct(provider, eventSourceId);
    }
}
