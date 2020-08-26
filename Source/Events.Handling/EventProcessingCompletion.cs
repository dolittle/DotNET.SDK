// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessingCompletion"/>.
    /// </summary>
    [Singleton]
    public class EventProcessingCompletion : IEventProcessingCompletion
    {
        readonly ConcurrentDictionary<Type, List<EventHandlerType>> _eventHandlersByEventType = new ConcurrentDictionary<Type, List<EventHandlerType>>();
        readonly ConcurrentDictionary<CorrelationId, EventHandlersWaiter> _eventHandlersWaitersByScope = new ConcurrentDictionary<CorrelationId, EventHandlersWaiter>();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessingCompletion"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventProcessingCompletion(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void RegisterHandler(EventHandlerId eventHandler, IEnumerable<Type> eventTypes)
        {
            eventTypes.ForEach(_ =>
                {
                    var eventHandlerType = new EventHandlerType(eventHandler, _);
                    _eventHandlersByEventType.AddOrUpdate(_, new List<EventHandlerType> { eventHandlerType }, (_, v) =>
                        {
                            v.Add(eventHandlerType);
                            return v;
                        });
                });
        }

        /// <inheritdoc/>
        public void EventHandlerCompletedForEvent(CorrelationId correlationId, EventHandlerId eventHandlerId, Type type)
        {
            _logger.Debug("Event Handler {EventHandler} completed for Event {EventType} with correlation {CorrelationId}", eventHandlerId, type, correlationId);
            if (_eventHandlersWaitersByScope.ContainsKey(correlationId))
            {
                var waiter = _eventHandlersWaitersByScope[correlationId];
                waiter.Signal(new EventHandlerType(eventHandlerId, type));
                if (waiter.IsDone())
                {
                    _eventHandlersWaitersByScope.TryRemove(correlationId, out EventHandlersWaiter _);
                }
            }
        }

        /// <inheritdoc/>
        public Task Perform(CorrelationId correlationId, IEnumerable<IEvent> events, Action action)
        {
            if (_eventHandlersWaitersByScope.ContainsKey(correlationId)) throw new AlreadyPerformingEventProcessingCompletionForCorrelation(correlationId);
            _logger.Debug("Performing event processing completion {NumEvents} for correlation {Correlation}", events.Count(), correlationId);
            var tcs = new TaskCompletionSource<bool>();
            var eventHandlersForScope = new List<EventHandlerType>();
            events
                .Select(_ => _.GetType())
                .ForEach(eventType => _eventHandlersByEventType
                                        .Where(_ => _.Key == eventType)
                                        .SelectMany(_ => _.Value)
                                        .ForEach(eventHandlersForScope.Add));

            var waiter = new EventHandlersWaiter(correlationId, eventHandlersForScope, _logger);

            if (!_eventHandlersWaitersByScope.TryAdd(correlationId, waiter)) throw new AlreadyPerformingEventProcessingCompletionForCorrelation(correlationId);

            Task.Run(async () =>
            {
                try
                {
                    action();
                    _logger.Trace("EventProcessingCompletion.PERFORM LOOP Time for eventHandlers {CorrelationId}", correlationId);
                    _logger.Trace("EventProcessingCompletion.PERFORM LOOP Waiting for EventHandlers: {CorrelationId}", correlationId);
                    await waiter.Complete().ConfigureAwait(false);
                    _logger.Trace("EventProcessingCompletion.PERFORM LOOP EventHandler waiting is done {CorrelationId}", correlationId);

                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "An error occurred while performing event processing completion");
                }
                finally
                {
                    tcs.SetResult(true);
                }
            });

            return tcs.Task;
        }
    }
}
