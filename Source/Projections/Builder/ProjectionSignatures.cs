// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;



/// <summary>
/// Represents the signature of a key selector.
/// </summary>
/// <param name="keySelectorBuilder">The <see cref="KeySelectorBuilder"/>.</param>
public delegate KeySelector KeySelectorSignature(KeySelectorBuilder keySelectorBuilder);

/// <summary>
/// Represents the signature of a key selector.
/// </summary>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to select the key from.</typeparam>
/// <param name="keySelectorBuilder">The <see cref="KeySelectorBuilder{TEvent}"/>.</param>
public delegate KeySelector KeySelectorSignature<TEvent>(KeySelectorBuilder<TEvent> keySelectorBuilder)
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResult<TReadModel> ProjectionSignature<TReadModel>(TReadModel readModel, object @event, ProjectionContext projectionContext)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResult<TReadModel> ProjectionSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="eventContext">The <see cref="EventContext" />.</param>
public delegate ProjectionResult<TReadModel> ProjectionEventContextSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event, EventContext eventContext)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="eventContext">The <see cref="EventContext" />.</param>
public delegate ProjectionResult<TReadModel> ProjectionEventContextSignature<TReadModel>(TReadModel readModel, object @event, EventContext eventContext)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
public delegate ProjectionResult<TReadModel> ProjectionEventSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
public delegate ProjectionResult<TReadModel> ProjectionEventSignature<TReadModel>(TReadModel readModel, object @event)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void ProjectionMethodEventSignature<TReadModel>(TReadModel instance, object @event)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void ProjectionMethodSignature<TReadModel>(TReadModel instance, object @event, ProjectionContext projectionContext)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void ProjectionMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="eventContext">The <see cref="EventContext" />.</param>
public delegate void ProjectionMethodEventContextSignature<TReadModel>(TReadModel instance, object @event, EventContext eventContext)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="eventContext">The <see cref="EventContext" />.</param>
public delegate void ProjectionMethodEventContextSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, EventContext eventContext)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="eventContext">The <see cref="EventContext" />.</param>
public delegate void ProjectionMethodEventSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType ProjectionResultTypeSignature<TReadModel>(TReadModel instance, object @event, ProjectionContext projectionContext)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType ProjectionResultTypeSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType ProjectionResultTypeEventContextSignature<TReadModel>(TReadModel instance, object @event, EventContext projectionContext)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType ProjectionResultTypeEventContextSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event,
    EventContext projectionContext)
    where TReadModel : ReadModel, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType ProjectionResultTypeEventSignature<TReadModel>(TReadModel instance, object @event)
    where TReadModel : ReadModel, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType ProjectionResultTypeEventSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event)
    where TReadModel : ReadModel, new()
    where TEvent : class;
