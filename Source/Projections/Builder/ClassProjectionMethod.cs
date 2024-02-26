// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// An implementation of <see cref="IProjectionMethod{TProjection}" /> that invokes a method on a projection instance for an event of a specific type.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
public class ClassProjectionMethod<TProjection> : IProjectionMethod<TProjection>
    where TProjection : class, new()
{
    readonly TaskResultProjectionMethodSignature<TProjection> _method;
    readonly EventType _eventType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassProjectionMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskProjectionMethodSignature{TProjection}"/> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector"/> for selecting the key.</param>
    public ClassProjectionMethod(TaskProjectionMethodSignature<TProjection> method, EventType eventType, KeySelector keySelector)
        : this(
            async (TProjection instanceAndReadModel, object @event, ProjectionContext context) =>
            {
                await method(instanceAndReadModel, @event, context).ConfigureAwait(false);
                return ProjectionResultType.Replace;
            },
            eventType,
            keySelector)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassProjectionMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskResultProjectionMethodSignature{TProjection}"/> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector"/> for selecting the key.</param>
    public ClassProjectionMethod(TaskResultProjectionMethodSignature<TProjection> method, EventType eventType, KeySelector keySelector)
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
            (TProjection instanceAndReadModel, object @event, ProjectionContext context) =>
            {
                method(instanceAndReadModel, @event, context);
                return ProjectionResultType.Replace;
            },
            eventType,
            keySelector)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassProjectionMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncResultProjectionMethodSignature{TEvent, TProjection}" /> method to invoke.</param>
    /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector"/> for selecting the key.</param>
    public ClassProjectionMethod(SyncResultProjectionMethodSignature<TProjection> method, EventType eventType, KeySelector keySelector)
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
    public async Task<Try<ProjectionResult<TProjection>>> TryOn(TProjection readModel, object @event, ProjectionContext context)
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
