// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates
{
    /// <summary>
    /// Represents a factory to use to create instances of <see cref="AggregateRootMockOperations{TAggregate}"/>.
    /// </summary>
    public static class AggregateRootMock
    {
        /// <summary>
        /// Creates a new <see cref="AggregateRootMockOperations{TAggregate}"/> with the given <see cref="EventSourceId"/>.
        /// </summary>
        /// <param name="eventSourceId">The event source id to use for the mock.</param>
        /// <typeparam name="TAggregate">The type of the <see cref="AggregateRoot"/> to create a mock for.</typeparam>
        /// <returns>The <see cref="AggregateRootMockOperations{TAggregate}"/> to use for testing.</returns>
        public static AggregateRootMockOperations<TAggregate> Of<TAggregate>(EventSourceId eventSourceId)
            where TAggregate : AggregateRoot
            => new AggregateRootMockOperations<TAggregate>(eventSourceId);
    }
}