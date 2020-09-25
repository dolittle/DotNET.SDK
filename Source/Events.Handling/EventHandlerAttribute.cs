// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Decorates a class to indicate the Event Handler Id of the Event Handler class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerAttribute"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The unique identifier of the event handler.</param>
        /// <param name="partitioned">Whether the event handler is partitioned.</param>
        /// <param name="scopeId">The scope that the event handler handles events in.</param>
        public EventHandlerAttribute(string eventHandlerId, bool partitioned = true, string scopeId = default)
        {
            Identifier = Guid.Parse(eventHandlerId);
            Partitioned = partitioned;
            ScopeId = scopeId == default ? ScopeId.Default : new ScopeId(Guid.Parse(scopeId));
        }

        /// <summary>
        /// Gets the unique identifier for this event handler.
        /// </summary>
        public EventHandlerId Identifier { get; }

        /// <summary>
        /// Gets a value indicating whether this event handler is partitioned.
        /// </summary>
        public bool Partitioned { get; }

        /// <summary>
        /// Gets the <see cref="Events.ScopeId" />.
        /// </summary>
        public ScopeId ScopeId { get; }
    }
}
