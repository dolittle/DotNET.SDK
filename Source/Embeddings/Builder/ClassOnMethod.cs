// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// An implementation of <see cref="IOnMethod{TProjection}" /> that invokes a method on a projection instance for an event of a specific type.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
public class ClassOnMethod<TProjection> : IOnMethod<TProjection>
    where TProjection : class, new()
{
    readonly TaskResultOnMethodSignature<TProjection> _method;
    readonly EventType _eventType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassOnMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskOnMethodSignature{TProjection}"/> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    public ClassOnMethod(TaskOnMethodSignature<TProjection> method, EventType eventType)
        : this(
            async (TProjection instanceAndReadModel, object @event, EmbeddingProjectContext context) =>
            {
                await method(instanceAndReadModel, @event, context).ConfigureAwait(false);
                return ProjectionResultType.Replace;
            },
            eventType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassOnMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskResultOnMethodSignature{TProjection}"/> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    public ClassOnMethod(TaskResultOnMethodSignature<TProjection> method, EventType eventType)
    {
        _method = method;
        _eventType = eventType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassOnMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncOnMethodSignature{TEvent, TProjection}" /> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    public ClassOnMethod(SyncOnMethodSignature<TProjection> method, EventType eventType)
        : this(
            (TProjection instanceAndReadModel, object @event, EmbeddingProjectContext context) =>
            {
                method(instanceAndReadModel, @event, context);
                return ProjectionResultType.Replace;
            },
            eventType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassOnMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncResultOnMethodSignature{TEvent, TProjection}" /> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    public ClassOnMethod(SyncResultOnMethodSignature<TProjection> method, EventType eventType)
        : this(
            (TProjection instanceAndReadModel, object @event, EmbeddingProjectContext context)
                => Task.FromResult(method(instanceAndReadModel, @event, context)),
            eventType)
    {
    }

    /// <inheritdoc/>
    public EventType GetEventType(IEventTypes eventTypes)
        => _eventType;

    /// <inheritdoc/>
    public async Task<Try<ProjectionResult<TProjection>>> TryOn(TProjection readModel, object @event, EmbeddingProjectContext context)
    {
        var resultType = await _method(readModel, @event, context).TryTask().ConfigureAwait(false);
        return resultType switch
        {
            { Success: true, Result: ProjectionResultType.Delete } => ProjectionResult<TProjection>.Delete,
            { Success: true, Result: ProjectionResultType.Replace } => new Try<ProjectionResult<TProjection>>(readModel),
            _ => resultType.Exception
        };
    }
}