// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Defines a projection.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public interface IProjection<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Gets the unique identifier for projection - <see cref="ProjectionId" />.
        /// </summary>
        ProjectionId Identifier { get; }

        /// <summary>
        /// Gets the scope the projection is in.
        /// </summary>
        ScopeId ScopeId { get; }

        /// <summary>
        /// Gets the initial <typeparamref name="TReadModel"/> read model state.
        /// </summary>
        TReadModel InitialState { get; }

        /// <summary>
        /// Gets the event types identified by its artifact that is handled by this event handler.
        /// </summary>
        IEnumerable<EventSelector> Events { get; }

        /// <summary>
        /// Handle an event and update a readmodel.
        /// </summary>
        /// <param name="readModel">The read model to update.</param>
        /// <param name="event">The event to handle.</param>
        /// <param name="eventType">The artifact representign the event type.</param>
        /// <param name="context">The context of the projection.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="ProjectionResult{TReadModel}"/>.</returns>
        Task<ProjectionResult<TReadModel>> On(TReadModel readModel, object @event, EventType eventType, ProjectionContext context, CancellationToken cancellation);
    }
}
