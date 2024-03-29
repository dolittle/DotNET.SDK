// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// An implementation of <see cref="IProjectionMethod{TReadModel}" />.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
public class ProjectionMethod<TReadModel> : IProjectionMethod<TReadModel>
    where TReadModel : ReadModel, new()
{
    readonly ProjectionSignature<TReadModel> _method;
    readonly EventType _eventType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionMethod{TReadModel}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskProjectionSignature{TReadModel}" />.</param>
    /// <param name="keySelector">The <see cref="KeySelector" />.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    public ProjectionMethod(ProjectionSignature<TReadModel> method, KeySelector keySelector, EventType eventType)
    {
        _method = method;
        _eventType = eventType;
        KeySelector = keySelector;
    }

    /// <inheritdoc/>
    public KeySelector KeySelector { get; }

    /// <inheritdoc/>
    public EventType GetEventType(IEventTypes eventTypes) => _eventType;

    /// <inheritdoc/>
    public ProjectionResult<TReadModel> TryOn(TReadModel readModel, object @event, ProjectionContext context) => _method(readModel, @event, context);
}
