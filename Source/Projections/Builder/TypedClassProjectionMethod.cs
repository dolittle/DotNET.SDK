// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
    readonly SyncResultProjectionMethodSignature<TProjection, TEvent> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassProjectionMethod{TProjection, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskResultProjectionMethodSignature{TProjection, TEvent}"/> method to invoke.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
    public TypedClassProjectionMethod(SyncResultProjectionMethodSignature<TProjection, TEvent> method, KeySelector keySelector)
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

    /// <inheritdoc/>
    public KeySelector KeySelector { get; }

    /// <inheritdoc/>
    public EventType GetEventType(IEventTypes eventTypes) => eventTypes.GetFor(typeof(TEvent));

    /// <inheritdoc/>
    public ProjectionResult<TProjection> TryOn(TProjection readModel, object @event, ProjectionContext context)
    {
        if (@event is not TEvent typedEvent)
        {
            throw new TypedProjectionMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType());
        }

        var resultType = _method(readModel, typedEvent, context);

        return resultType switch
        {
            ProjectionResultType.Delete => ProjectionResult<TProjection>.Delete,
            ProjectionResultType.Replace => new Try<ProjectionResult<TProjection>>(readModel),
            ProjectionResultType.Keep => ProjectionResult<TProjection>.Keep,
            _ => ProjectionResult<TProjection>.Keep
        };
    }
}
