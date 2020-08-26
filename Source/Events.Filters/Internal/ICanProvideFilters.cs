// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// Represents a system that is capable of providing implementations of event filters.
    /// </summary>
    /// <typeparam name="TFilterType">The type of event filter to provide.</typeparam>
    /// <typeparam name="TEventType">The event type that the filter can handle.</typeparam>
    /// <typeparam name="TFilterResult">The type of filter result that the filter returns.</typeparam>
    public interface ICanProvideFilters<TFilterType, TEventType, TFilterResult>
        where TFilterType : ICanFilter<TEventType, TFilterResult>
        where TEventType : IEvent
    {
        /// <summary>
        /// Provides implementations of <typeparamref name="TFilterType"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of type <typeparamref name="TFilterType"/>.</returns>
        IEnumerable<TFilterType> Provide();
    }
}