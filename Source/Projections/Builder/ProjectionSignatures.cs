// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

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
public delegate Task<ProjectionResult<TReadModel>> TaskProjectionSignature<TReadModel>(TReadModel readModel, object @event, ProjectionContext projectionContext)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate Task<ProjectionResult<TReadModel>> TaskProjectionSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResult<TReadModel> SyncProjectionSignature<TReadModel>(TReadModel readModel, object @event, ProjectionContext projectionContext)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResult<TReadModel> SyncProjectionSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate Task TaskProjectionMethodSignature<TProjection>(TProjection instance, object @event, ProjectionContext projectionContext)
    where TProjection : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate Task TaskProjectionMethodSignature<TProjection, TEvent>(TProjection instance, TEvent @event, ProjectionContext projectionContext)
    where TProjection : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate Task<ProjectionResultType> TaskResultProjectionMethodSignature<TProjection>(TProjection instance, object @event, ProjectionContext projectionContext)
    where TProjection : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate Task<ProjectionResultType> TaskResultProjectionMethodSignature<TProjection, TEvent>(TProjection instance, TEvent @event, ProjectionContext projectionContext)
    where TProjection : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void SyncProjectionMethodSignature<TProjection>(TProjection instance, object @event, ProjectionContext projectionContext)
    where TProjection : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void SyncProjectionMethodSignature<TProjection, TEvent>(TProjection instance, TEvent @event, ProjectionContext projectionContext)
    where TProjection : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType SyncResultProjectionMethodSignature<TProjection>(TProjection instance, object @event, ProjectionContext projectionContext)
    where TProjection : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType SyncResultProjectionMethodSignature<TProjection, TEvent>(TProjection instance, TEvent @event, ProjectionContext projectionContext)
    where TProjection : class, new()
    where TEvent : class;