// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// An implementation of <see cref="IEventHandler" />.
    /// </summary>
    public class EventHandler : IEventHandler
    {
        public EventHandler(
            EventHandlerId identifier,
            ScopeId scopeId,
            bool partitioned,
            IDictionary<EventType, Method> signature
        )
        {
            Identifier = identifier;
            ScopeId = scopeId;
            Partitioned = partitioned;
        }

        /// <inheritdoc/>
        public EventHandlerId Identifier { get; }

        /// <inheritdoc/>
        public ScopeId ScopeId { get; }

        /// <inheritdoc/>
        public bool Partitioned { get; }

        /// <inheritdoc/>
        public IEnumerable<EventType> HandledEvents => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public Task Handle(object @event, EventType eventType, EventContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
