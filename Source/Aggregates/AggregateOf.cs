// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateOf{TAggregateRoot}"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The <see cref="Type"/> of the <see cref="AggregateRoot"/> class.</typeparam>
    public class AggregateOf<TAggregateRoot> : IAggregateOf<TAggregateRoot>
        where TAggregateRoot : AggregateRoot
    {
        readonly IAggregates _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateOf{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="aggregates">The <see cref="IAggregates"/>.</param>
        public AggregateOf(IAggregates aggregates)
        {
            _aggregates = aggregates;
        }

        /// <inheritdoc />
        public IAggregateRootOperations<TAggregateRoot> Get(EventSourceId eventSourceId)
            => _aggregates.Get<TAggregateRoot>(eventSourceId);
    }
}