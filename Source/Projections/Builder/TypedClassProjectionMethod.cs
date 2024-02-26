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
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
public class TypedClassProjectionMethod<TProjection, TEvent> : IProjectionMethod<TProjection>
    where TProjection : class, new()
    where TEvent : class
{
    readonly TaskResultProjectionMethodSignature<TProjection, TEvent> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassProjectionMethod{TProjection, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskProjectionMethodSignature{TProjection, TEvent}"/> method to invoke.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
    public TypedClassProjectionMethod(TaskProjectionMethodSignature<TProjection, TEvent> method, KeySelector keySelector)
        : this(
            async (TProjection instanceAndReadModel, TEvent @event, ProjectionContext context) =>
            {
                await method(instanceAndReadModel, @event, context).ConfigureAwait(false);
                return ProjectionResultType.Replace;
            },
            keySelector)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassProjectionMethod{TProjection, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskResultProjectionMethodSignature{TProjection, TEvent}"/> method to invoke.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
    public TypedClassProjectionMethod(TaskResultProjectionMethodSignature<TProjection, TEvent> method, KeySelector keySelector)
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
            (TProjection instanceAndReadModel, TEvent @event, ProjectionContext context) =>
            {
                method(instanceAndReadModel, @event, context);
                return ProjectionResultType.Replace;
            },
            keySelector)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassProjectionMethod{TProjection, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncResultProjectionMethodSignature{TProjection, TEvent}" /> method to invoke.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
    public TypedClassProjectionMethod(
        SyncResultProjectionMethodSignature<TProjection, TEvent> method,
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
    public async Task<Try<ProjectionResult<TProjection>>> TryOn(TProjection readModel, object @event, ProjectionContext context)
    {
        if (@event is TEvent typedEvent)
        {
            var resultType = await _method(readModel, typedEvent, context).TryTask().ConfigureAwait(false);
            return resultType switch
            {
                { Success: true, Result: ProjectionResultType.Delete } => ProjectionResult<TProjection>.Delete,
                { Success: true, Result: ProjectionResultType.Replace } => new Try<ProjectionResult<TProjection>>(readModel),
                _ => resultType.Exception
            };
        }

        return new TypedProjectionMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType());
    }
}
