// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate Task<ProjectionResult<TReadModel>> TaskOnSignature<TReadModel>(TReadModel readModel, object @event, EmbeddingProjectContext context)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate Task<ProjectionResult<TReadModel>> TaskOnSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event, EmbeddingProjectContext context)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate ProjectionResult<TReadModel> SyncOnSignature<TReadModel>(TReadModel readModel, object @event, EmbeddingProjectContext context)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event to handle.</typeparam>
/// <param name="readModel">The read model.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate ProjectionResult<TReadModel> SyncOnSignature<TReadModel, TEvent>(TReadModel readModel, TEvent @event, EmbeddingProjectContext context)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate Task TaskOnMethodSignature<TReadModel>(TReadModel instance, object @event, EmbeddingProjectContext context)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the embeddings.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate Task TaskOnMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, EmbeddingProjectContext context)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate Task<ProjectionResultType> TaskResultOnMethodSignature<TReadModel>(TReadModel instance, object @event, EmbeddingProjectContext context)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the embeddings.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate Task<ProjectionResultType> TaskResultOnMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, EmbeddingProjectContext context)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the embeddings.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate void SyncOnMethodSignature<TReadModel>(TReadModel instance, object @event, EmbeddingProjectContext context)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the embeddings.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate void SyncOnMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, EmbeddingProjectContext context)
    where TReadModel : class, new()
    where TEvent : class;

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the embeddings.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate ProjectionResultType SyncResultOnMethodSignature<TReadModel>(TReadModel instance, object @event, EmbeddingProjectContext context)
    where TReadModel : class, new();

/// <summary>
/// Represents the signature for an embeddings on-method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the embeddings.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
/// <param name="instance">The instance of the embeddings to invoke the method on.</param>
/// <param name="event">The event to handle.</param>
/// <param name="context">The <see cref="EmbeddingProjectContext" />.</param>
public delegate ProjectionResultType SyncResultOnMethodSignature<TReadModel, TEvent>(TReadModel instance, TEvent @event, EmbeddingProjectContext context)
    where TReadModel : class, new()
    where TEvent : class;