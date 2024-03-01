// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void SyncProjectionMethodSignature<TReadModel>(TReadModel instance, object @event, ProjectionContext projectionContext)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate void SyncProjectionMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate TReadModel SyncProjectionValueSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType SyncResultProjectionMethodSignature<TReadModel>(TReadModel instance, object @event, ProjectionContext projectionContext)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for a projection on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the projection to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="projectionContext">The <see cref="ProjectionContext" />.</param>
public delegate ProjectionResultType SyncResultProjectionMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, ProjectionContext projectionContext)
    where TReadModel : class, new()
    where TEvent : class;
