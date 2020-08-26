// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Logging;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Represents a waiter for event handlers.
    /// </summary>
    public class EventHandlersWaiter
    {
        readonly object _lockObject = new object();
        readonly IList<EventHandlerType> _eventTypeHandlers;
        readonly CorrelationId _correlationId;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersWaiter"/> class.
        /// </summary>
        /// <param name="correlationId">The <see cref="CorrelationId" />.</param>
        /// <param name="eventTypeHandlers">The <see cref="IEnumerable{T}" /> of <see cref="EventHandlerType" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlersWaiter(CorrelationId correlationId, IEnumerable<EventHandlerType> eventTypeHandlers, ILogger logger)
        {
            _eventTypeHandlers = new List<EventHandlerType>(eventTypeHandlers);
            _correlationId = correlationId;
            _logger = logger;
        }

        /// <summary>
        /// Signal for a specific event type.
        /// </summary>
        /// <param name="eventHandlerType"><see cref="EventHandlerType"/> to signal for.</param>
        public void Signal(EventHandlerType eventHandlerType)
        {
            lock (_lockObject)
            {
                _eventTypeHandlers.Remove(eventHandlerType);
            }
        }

        /// <summary>
        /// Check whether or not we're done or not.
        /// </summary>
        /// <returns>true if we're done, false if not.</returns>
        public bool IsDone()
        {
            lock (_lockObject)
            {
                return _eventTypeHandlers.Count == 0;
            }
        }

        /// <summary>
        /// Wait for all event handlers to be done.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Complete()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Task.Run(async () =>
            {
                int num = 1500;
                while (!IsDone())
                {
                    await Task.Delay(20).ConfigureAwait(false);
                    if (num-- == 0)
                    {
                        var waitingFor = string.Join(System.Environment.NewLine, _eventTypeHandlers.Select(_ => _.Type.ToString()));
                        var handlers = string.Join(System.Environment.NewLine, _eventTypeHandlers.Select(_ => _.EventHandler));
                        _logger.Debug("Waiting timed out for {WaitingFor} in event handlers {EventHandlers}. Completing waiter with correlation {CorrelationId}", waitingFor, handlers, _correlationId);
                        break;
                    }
                }

                tcs.SetResult(true);
            });
            return tcs.Task;
        }
    }
}
