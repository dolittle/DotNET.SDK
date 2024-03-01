// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// An implementation of <see cref="IProjectionMethod{TReadModel}" /> for a projection method on an event of a specific type.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
public class TypedProjectionMethod<TReadModel, TEvent> : IProjectionMethod<TReadModel>
    where TReadModel : class, new()
    where TEvent : class
{
    readonly SyncProjectionSignature<TReadModel, TEvent> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedProjectionMethod{TReadModel, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskProjectionSignature{TReadModel, TEvent}" />.</param>
    /// <param name="keySelector">The <see cref="Projections.KeySelector" />.</param>
    public TypedProjectionMethod(SyncProjectionSignature<TReadModel, TEvent> method, KeySelector keySelector)
    {
        _method = method;
        KeySelector = keySelector;
    }

    /// <inheritdoc/>
    public KeySelector KeySelector { get; }

    /// <inheritdoc/>
    public EventType GetEventType(IEventTypes eventTypes)
        => eventTypes.GetFor(typeof(TEvent));

    /// <inheritdoc/>
    public ProjectionResult<TReadModel> TryOn(TReadModel readModel, object @event, ProjectionContext context)
    {
        if (@event is not TEvent typedEvent)
        {
        throw new TypedProjectionMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType());
        }

        return _method(readModel, typedEvent, context);

    }
}
