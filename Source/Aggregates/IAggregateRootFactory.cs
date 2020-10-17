// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Defines a factory for <see cref="AggregateRoot" />.
    /// </summary>
    public interface IAggregateRootFactory
    {
        /// <summary>
        /// Creates an instance of the aggregate root.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The <see cref="Type" /> of the aggregate root.</typeparam>
        /// <param name="eventSourceId">The event source id.</param>
        /// <returns>The <typeparamref name="TAggregateRoot"/>.</returns>
        TAggregateRoot Create<TAggregateRoot>(EventSourceId eventSourceId)
            where TAggregateRoot : AggregateRoot;
    }
}
