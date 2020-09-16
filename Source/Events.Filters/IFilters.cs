// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Defines a system that knows about filters.
    /// </summary>
    public interface IFilters
    {
        /// <summary>
        /// Register a <see cref="IFilterProcessor" /> for processing events for filtering.
        /// </summary>
        /// <param name="filter">The <see cref="IFilterProcessor "/>.</param>
        /// <param name="cancellation">The optional <see cref="CancellationToken" />.</param>
        void Register(IFilterProcessor filter, CancellationToken cancellation = default);
    }
}
