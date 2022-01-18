// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// An implementation of <see cref="IOnMethod{TReadModel}" /> for a projection method on an event of a specific type.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
public class TypedOnMethod<TReadModel, TEvent> : IOnMethod<TReadModel>
    where TReadModel : class, new()
    where TEvent : class
{
    readonly TaskOnSignature<TReadModel, TEvent> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedOnMethod{TReadModel, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskOnSignature{TReadModel, TEvent}" />.</param>
    public TypedOnMethod(TaskOnSignature<TReadModel, TEvent> method)
    {
        _method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedOnMethod{TReadModel, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncOnSignature{TReadModel, TEvent}" />.</param>
    public TypedOnMethod(SyncOnSignature<TReadModel, TEvent> method)
        : this(
            (TReadModel readModel, TEvent @event, EmbeddingProjectContext context)
                => Task.FromResult(method(readModel, @event, context)))
    {
    }

    /// <inheritdoc/>
    public EventType GetEventType(IEventTypes eventTypes)
        => eventTypes.GetFor(typeof(TEvent));

    /// <inheritdoc/>
    public Task<Try<ProjectionResult<TReadModel>>> TryOn(TReadModel readModel, object @event, EmbeddingProjectContext context)
    {
        if (@event is TEvent typedEvent) return _method(readModel, typedEvent, context).TryTask();

        return Task.FromResult<Try<ProjectionResult<TReadModel>>>(new TypedOnMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType()));
    }
}