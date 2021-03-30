// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Defines an on method.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public interface IOnMethod<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Gets the <see cref="KeySelector" />.
        /// </summary>
        KeySelector KeySelector { get; }

        /// <summary>
        /// Invokes the on method.
        /// </summary>
        /// <param name="readModel">The read model.</param>
        /// <param name="event">The event.</param>
        /// <param name="context">The context of the projection.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="ProjectionResult{TReadModel}" />.</returns>
        Task<Try<ProjectionResult<TReadModel>>> TryOn(TReadModel readModel, object @event, ProjectionContext context);

        /// <summary>
        /// Gets the <see cref="EventType" />.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <returns>The <see cref="EventType" />.</returns>
        EventType GetEventType(IEventTypes eventTypes);
    }
}
