// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregates"/>.
/// </summary>
public class Aggregates : IAggregates
{
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregates"/> class.
    /// </summary>
    /// <param name="serviceProvider">The tenant scoped <see cref="IServiceProvider"/>.</param>
    public Aggregates(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public IAggregateRootOperations<TAggregateRoot> Get<TAggregateRoot>(EventSourceId eventSourceId)
        where TAggregateRoot : AggregateRoot
        => ActivatorUtilities.CreateInstance<AggregateRootOperations<TAggregateRoot>>(_serviceProvider, eventSourceId);

    /// <inheritdoc />
    public IAggregateOf<TAggregateRoot> Of<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
        => new AggregateOf<TAggregateRoot>(this);
}
