// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// An implementation of <see cref="IOnMethod{TReadModel}" />.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
    public class OnMethod<TReadModel> : IOnMethod<TReadModel>
        where TReadModel : class, new()
    {
        readonly TaskOnSignature<TReadModel> _method;
        readonly EventType _eventType;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnMethod{TReadModel}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
        /// <param name="eventType">The <see cref="EventType" />.</param>
        public OnMethod(TaskOnSignature<TReadModel> method, EventType eventType)
        {
            _method = method;
            _eventType = eventType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnMethod{TReadModel}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
        /// <param name="eventType">The <see cref="EventType" />.</param>
        public OnMethod(SyncOnSignature<TReadModel> method, EventType eventType)
            : this(
                (readModel, @event, context) => Task.FromResult(method(readModel, @event, context)),
                eventType)
        {
        }

        /// <inheritdoc/>
        public EventType GetEventType(IEventTypes eventTypes)
            => _eventType;

        /// <inheritdoc/>
        public Task<Try<ProjectionResult<TReadModel>>> TryOn(TReadModel readModel, object @event, EmbeddingProjectContext context)
            => _method(readModel, @event, context).TryTask();
    }
}
