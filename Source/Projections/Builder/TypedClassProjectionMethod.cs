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
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    public class TypedClassProjectionMethod<TProjection, TEvent> : IOnMethod<TProjection>
        where TProjection : class, new()
        where TEvent : class
    {
        readonly TaskProjectionMethodSignature<TProjection, TEvent> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassProjectionMethod{TProjection, TEvent}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="TaskProjectionMethodSignature{TProjection, TEvent}"/> method to invoke.</param>
        /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
        public TypedClassProjectionMethod(
            TaskProjectionMethodSignature<TProjection, TEvent> method,
            KeySelector keySelector)
        {
            _method = method;
            KeySelector = keySelector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassProjectionMethod{TProjection, TEvent}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="SyncProjectionMethodSignature{TProjection, TEvent}" /> method to invoke.</param>
        /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
        public TypedClassProjectionMethod(
            SyncProjectionMethodSignature<TProjection, TEvent> method,
            KeySelector keySelector)
            : this(
                (TProjection instanceAndReadModel, TEvent @event, ProjectionContext context)
                    => Task.FromResult(method(instanceAndReadModel, @event, context)),
                keySelector)
        {
        }

        /// <inheritdoc/>
        public KeySelector KeySelector { get; }

        /// <inheritdoc/>
        public EventType GetEventType(IEventTypes eventTypes)
            => eventTypes.GetFor(typeof(TEvent));

        /// <inheritdoc/>
        public Task<Try<ProjectionResult<TProjection>>> TryOn(TProjection readModel, object @event, ProjectionContext context)
        {
            if (@event is TEvent typedEvent) return _method(readModel, typedEvent, context).TryTask();

            return Task.FromResult<Try<ProjectionResult<TProjection>>>(new TypedProjectionMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType()));
        }
    }
}
