// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Testing.Events;
using Dolittle.SDK.Testing.Events.Store;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dolittle.SDK.Testing.Aggregates
{
    /// <summary>
    /// Represents a mock of <see cref="IAggregateRootOperations{TAggregate}"/> that allows you to perform operations on
    /// an <see cref="AggregateRoot"/> with a given set of already applied events, and to capture the events that is
    /// applied as a result of the operation.
    /// </summary>
    /// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
    public class AggregateRootMockOperations<TAggregate>
        where TAggregate : AggregateRoot
    {
        readonly EventStoreMock _eventStore;
        readonly List<Func<TAggregate, Task>> _queuedActions = new List<Func<TAggregate, Task>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootMockOperations{TAggregate}"/> class.
        /// </summary>
        /// <param name="eventSourceId">The event source id of the mock.</param>
        public AggregateRootMockOperations(EventSourceId eventSourceId)
        {
            var eventTypes = new EventTypesMock();

            _eventStore = EventStoreMock.Create(eventTypes);

            Operations = new AggregateRootOperations<TAggregate>(
                eventSourceId,
                _eventStore,
                eventTypes,
                new AggregateRoots(NullLogger.Instance),
                NullLogger.Instance);
        }

        /// <summary>
        /// Gets an <see cref="IAggregateRootOperations{TAggregate}"/> that can be used to perform operations on the mock.
        /// </summary>
        public IAggregateRootOperations<TAggregate> Operations { get; }

        /// <summary>
        /// Adds events to the mock that will be used to rehydrate the state of the <see cref="AggregateRoot"/> by
        /// calling the On()-methods before performing actions.
        /// </summary>
        /// <param name="events">The events to add.</param>
        /// <returns>The <see cref="AggregateRootMockOperations{TAggregate}"/> for continuation.</returns>
        public AggregateRootMockOperations<TAggregate> WithEvents(params object[] events)
        {
            _eventStore.EventsToFetch.AddRange(events);
            return this;
        }

        /// <summary>
        /// Schedules an operation to perform on the <see cref="AggregateRoot"/>.
        /// </summary>
        /// <remarks>
        /// The action will be called when <see cref="GetAppliedEvents"/> is called.
        /// </remarks>
        /// <param name="method">The operation to schedule.</param>
        /// <returns>The <see cref="AggregateRootMockOperations{TAggregate}"/> for continuation.</returns>
        public AggregateRootMockOperations<TAggregate> Perform(Action<TAggregate> method)
            => Perform((aggregate) =>
            {
                method(aggregate);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Schedules an operation to perform on the <see cref="AggregateRoot"/>.
        /// </summary>
        /// <remarks>
        /// The action will be called when <see cref="GetAppliedEvents"/> is called.
        /// </remarks>
        /// <param name="method">The operation to schedule.</param>
        /// <returns>The <see cref="AggregateRootMockOperations{TAggregate}"/> for continuation.</returns>
        public AggregateRootMockOperations<TAggregate> Perform(Func<TAggregate, Task> method)
        {
            _queuedActions.Add(method);
            return this;
        }

        /// <summary>
        /// Performs the scheduled operation, and returns the events that was applied by the <see cref="AggregateRoot"/>
        /// while performing the operations.
        /// </summary>
        /// <returns>A <see cref="Task"/> that, when resolved returns the applied events.</returns>
        public async Task<IList<object>> GetAppliedEvents()
        {
            await PerformAllQueuedActions().ConfigureAwait(false);
            return _eventStore.CommittedEvents.Select(_ => _.Content).ToList();
        }

        async Task PerformAllQueuedActions()
        {
            foreach (var action in _queuedActions)
            {
                await Operations.Perform(action).ConfigureAwait(false);
            }
        }
    }
}