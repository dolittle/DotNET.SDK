// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Events.Filters.EventHorizon;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Defines a manager that deals with registering event filters with the Runtime.
    /// </summary>
    public interface IRegisterFilters
    {
        /// <summary>
        /// Registers a filter with the Runtime.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the filter.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the filter will run.</param>
        /// <param name="filter">The <see cref="ICanFilterEvents"/> to register.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> representing the execution of the filter.</returns>
        Task Register(FilterId id, ScopeId scope, ICanFilterEvents filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a partitioned filter with the Runtime.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the filter.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the filter will run.</param>
        /// <param name="filter">The <see cref="ICanFilterEventsWithPartition"/> to register.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> representing the execution of the filter.</returns>
        Task Register(FilterId id, ScopeId scope, ICanFilterEventsWithPartition filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a public filter with the Runtime.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the filter.</param>
        /// <param name="filter">The <see cref="ICanFilterPublicEvents"/> to register.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> representing the execution of the filter.</returns>
        Task Register(FilterId id, ICanFilterPublicEvents filter, CancellationToken cancellationToken = default);
    }
}