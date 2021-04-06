// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Defines an interface for working directly with Projections.
    /// </summary>
    public interface IProjections
    {
        /// <summary>
        /// Gets a projection state.
        /// </summary>
        /// <param name="key">THe <see cref="Key" /> of the projection.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TProjection}" />.</returns>
        Task<CurrentState<TProjection>> Get<TProjection>(Key key, CancellationToken cancellation = default)
            where TProjection : class, new();

        /// <summary>
        /// Gets all projection state.
        /// </summary>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <see cref="CurrentState{TProjection}" />.</returns>
        Task<IEnumerable<CurrentState<TProjection>>> GetAll<TProjection>(CancellationToken cancellation = default)
            where TProjection : class, new();
    }
}
