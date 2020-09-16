// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Represents a building event handlers.
    /// </summary>
    public class EventHandlerBuilder
    {
        readonly EventHandlerId _eventHandlerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        public EventHandlerBuilder(EventHandlerId eventHandlerId) => _eventHandlerId = eventHandlerId;

        /// <summary>
        /// Gets the <see cref="ScopeId" />.
        /// </summary>
        public ScopeId ScopeId { get; private set; } = ScopeId.Default;

        /// <summary>
        /// Gets a value indicating whether the event handler is partitioned.
        /// </summary>
        public bool IsPartitioned { get; private set; } = true;

        /// <summary>
        /// Defines the event handler to be partitioned - this is default for an event handler.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerBuilder Partitioned()
        {
            IsPartitioned = true;
            return this;
        }

        /// <summary>
        /// Defines the event handler to be unpartitioned. By default it will be partitioned.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerBuilder Unpartitioned()
        {
            IsPartitioned = false;
            return this;
        }

        /// <summary>
        /// Defines the event handler to operate on a specific <see cref="ScopeId" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerBuilder InScope(ScopeId scopeId)
        {
            ScopeId = scopeId;
            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="method">The <see cref="EventHandlerSignature{T}" />.</param>
        public void Handle<T>(EventHandlerSignature<T> method)
            where T : class
        {
        }
    }

}