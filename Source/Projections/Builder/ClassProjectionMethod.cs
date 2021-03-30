// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// An implementation of <see cref="IOnMethod{TProjection}" /> that invokes a method on a projection instance for an event of a specific type.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    public class ClassProjectionMethod<TProjection> : IOnMethod<TProjection>
        where TProjection : class, new()
    {
        readonly TaskProjectionMethodSignature<TProjection> _method;
        readonly EventType _eventType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassProjectionMethod{TEventHandler}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="TaskProjectionMethodSignature{TProjection}"/> method to invoke.</param>
        /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
        /// <param name="keySelector">The <see cref="Projections.KeySelector"/> for selecting the key.</param>
        public ClassProjectionMethod(TaskProjectionMethodSignature<TProjection> method, EventType eventType, KeySelector keySelector)
        {
            _method = method;
            KeySelector = keySelector;
            _eventType = eventType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassProjectionMethod{TEventHandler}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="SyncProjectionMethodSignature{TEvent, TProjection}" /> method to invoke.</param>
        /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
        /// <param name="keySelector">The <see cref="Projections.KeySelector"/> for selecting the key.</param>
        public ClassProjectionMethod(SyncProjectionMethodSignature<TProjection> method, EventType eventType, KeySelector keySelector)
            : this(
                (TProjection instanceAndReadModel, object @event, ProjectionContext context)
                    => Task.FromResult(method(instanceAndReadModel, @event, context)),
                eventType,
                keySelector)
        {
        }

        /// <inheritdoc/>
        public KeySelector KeySelector { get; }

        /// <inheritdoc/>
        public EventType GetEventType(IEventTypes eventTypes)
            => _eventType;

        /// <inheritdoc/>
        public Task<Try<ProjectionResult<TProjection>>> TryOn(TProjection readModel, object @event, ProjectionContext context)
            => _method(readModel, @event, context).TryTask();
    }
}
