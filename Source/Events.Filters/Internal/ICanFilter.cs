// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// Defines a system that can filter instances of <typeparamref name="TEventType"/> to a stream.
    /// </summary>
    /// <typeparam name="TEventType">The event type that the filter can handle.</typeparam>
    /// <typeparam name="TFilterResult">The type of filter result that the filter returns.</typeparam>
    public interface ICanFilter<TEventType, TFilterResult>
        where TEventType : IEvent
    {
        /// <summary>
        /// Method that will be called to determine whether an <typeparamref name="TEventType"/> should be part of the stream or not.
        /// </summary>
        /// <param name="event">The <typeparamref name="TEventType"/> to filter.</param>
        /// <param name="context">The <see cref="EventContext"/> of the event to filter.</param>
        /// <returns>A <see cref="Task{T}"/> of type <typeparamref name="TFilterResult"/> representing the result of the asynchronous operation.</returns>
        Task<TFilterResult> Filter(TEventType @event, EventContext context);
    }
}