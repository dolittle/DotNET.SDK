// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbStreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Defines a system that can convert a request for processing an event to <see cref="EventContext" /> and the CLR instance of the event.
    /// </summary>
    public interface IEventProcessingRequestConverter
    {
        /// <summary>
        /// Gets the <see cref="EventContext" /> from the <see cref="PbCommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="PbCommittedEvent" />.</param>
        /// <returns>The derived <see cref="EventContext" />.</returns>
        EventContext GetEventContext(PbCommittedEvent @event);

        /// <summary>
        /// Gets the <see cref="EventContext" /> from the <see cref="PbStreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="PbStreamEvent" />.</param>
        /// <returns>The derived <see cref="EventContext" />.</returns>
        EventContext GetEventContext(PbStreamEvent @event);

        /// <summary>
        /// Gets the event instance from the <see cref="PbCommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="PbCommittedEvent" />.</param>
        /// <returns>Gets the event instance.</returns>
        object GetCLREvent(PbCommittedEvent @event);

        /// <summary>
        /// Gets the event instance from the <see cref="PbStreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="PbStreamEvent" />.</param>
        /// <returns>Gets the event instance.</returns>
        object GetCLREvent(PbStreamEvent @event);

        /// <summary>
        /// Gets the event instance from the <see cref="PbCommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="PbCommittedEvent" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
        /// <returns>Gets the event instance.</returns>
        T GetCLREvent<T>(PbCommittedEvent @event)
            where T : class;

        /// <summary>
        /// Gets the event instance from the <see cref="PbStreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="PbStreamEvent" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
        /// <returns>Gets the event instance.</returns>
        T GetCLREvent<T>(PbStreamEvent @event)
            where T : class;
    }
}