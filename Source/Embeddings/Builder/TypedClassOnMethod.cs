// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// An implementation of <see cref="IOnMethod{TEmbedding}" /> that invokes a method on an embedding instance for an event of a specific type.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
public class TypedClassOnMethod<TEmbedding, TEvent> : IOnMethod<TEmbedding>
    where TEmbedding : class, new()
    where TEvent : class
{
    readonly TaskResultOnMethodSignature<TEmbedding, TEvent> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassOnMethod{TEmbedding, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskOnMethodSignature{TEmbedding, TEvent}"/> method to invoke.</param>
    public TypedClassOnMethod(TaskOnMethodSignature<TEmbedding, TEvent> method)
        : this(
            async (TEmbedding instanceAndReadModel, TEvent @event, EmbeddingProjectContext context) =>
            {
                await method(instanceAndReadModel, @event, context).ConfigureAwait(false);
                return ProjectionResultType.Replace;
            })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassOnMethod{TEmbedding, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskResultOnMethodSignature{TEmbedding, TEvent}"/> method to invoke.</param>
    public TypedClassOnMethod(TaskResultOnMethodSignature<TEmbedding, TEvent> method)
    {
        _method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassOnMethod{TEmbedding, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncOnMethodSignature{TEmbedding, TEvent}" /> method to invoke.</param>
    public TypedClassOnMethod(SyncOnMethodSignature<TEmbedding, TEvent> method)
        : this(
            (TEmbedding instanceAndReadModel, TEvent @event, EmbeddingProjectContext context) =>
            {
                method(instanceAndReadModel, @event, context);
                return ProjectionResultType.Replace;
            })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassOnMethod{TEmbedding, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncResultOnMethodSignature{TEmbedding, TEvent}" /> method to invoke.</param>
    public TypedClassOnMethod(SyncResultOnMethodSignature<TEmbedding, TEvent> method)
        : this(
            (TEmbedding instanceAndReadModel, TEvent @event, EmbeddingProjectContext context)
                => Task.FromResult(method(instanceAndReadModel, @event, context)))
    {
    }

    /// <inheritdoc/>
    public EventType GetEventType(IEventTypes eventTypes)
        => eventTypes.GetFor(typeof(TEvent));

    /// <inheritdoc/>
    public async Task<Try<ProjectionResult<TEmbedding>>> TryOn(TEmbedding readModel, object @event, EmbeddingProjectContext context)
    {
        if (@event is TEvent typedEvent)
        {
            var resultType = await _method(readModel, typedEvent, context).TryTask().ConfigureAwait(false);
            return resultType switch
            {
                { Success: true, Result: ProjectionResultType.Delete } => ProjectionResult<TEmbedding>.Delete,
                { Success: true, Result: ProjectionResultType.Replace } => new Try<ProjectionResult<TEmbedding>>(readModel),
                _ => resultType.Exception
            };
        }

        return new TypedOnMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType());
    }
}