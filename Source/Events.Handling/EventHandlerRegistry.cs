// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Events.Handling.Internal;
using Dolittle.Resilience;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// An implementation of <see cref="IRegisterEventHandlers"/>.
    /// </summary>
    public class EventHandlerRegistry : IRegisterEventHandlers
    {
        readonly IEventHandlerProcessors _processors;
        readonly IAsyncPolicyFor<EventHandlerRegistry> _policy;
        readonly IEventProcessingCompletion _completion;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerRegistry"/> class.
        /// </summary>
        /// <param name="processors">The <see cref="IEventHandlerProcessors"/> that will be used to create instances of <see cref="EventHandlerProcessor{TEventType}"/>.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}"/> that defines reconnect policies for event handlers.</param>
        /// <param name="completion">The <see cref="IEventProcessingCompletion"/> that handles waiting for event handlers.</param>
        public EventHandlerRegistry(
            IEventHandlerProcessors processors,
            IAsyncPolicyFor<EventHandlerRegistry> policy,
            IEventProcessingCompletion completion)
        {
            _processors = processors;
            _policy = policy;
            _completion = completion;
        }

        /// <inheritdoc/>
        public Task Register<TEventType>(EventHandlerId id, ScopeId scope, bool partitioned, IEventHandler<TEventType> handler, CancellationToken cancellationToken = default)
            where TEventType : IEvent
            {
                _completion.RegisterHandler(id, handler.HandledEventTypes);
                return _processors.GetFor(id, scope, partitioned, handler).RegisterAndHandleForeverWithPolicy(_policy, cancellationToken);
            }
    }
}
