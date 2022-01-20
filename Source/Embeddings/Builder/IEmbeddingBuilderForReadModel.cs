// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Defines a builder for building an embedding for a read model.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the read model.</typeparam>
public interface IEmbeddingBuilderForReadModel<TReadModel>
    where TReadModel : class, new()
{
    /// <summary>
    /// Add the update method for resolving the received and current state of the embedding.
    /// The method should return a single event, that changes the current state more towards the received state.
    /// This method will be called until the received state and current state are equal.
    /// </summary>
    /// <param name="method">The <see cref="UpdateSignature{TReadModel}"/>.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> ResolveUpdateToEvents(UpdateSignature<TReadModel> method);
    /// <summary>
    /// Add the update method for resolving the received and current state of the embedding.
    /// The method should return an enumerable of  events, that change the current state more towards the received state.
    /// This method will be called until the received state and current state are equal.
    /// </summary>
    /// <param name="method">The <see cref="UpdateEnumerableReturnSignature{TReadModel}"/>.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> ResolveUpdateToEvents(UpdateEnumerableReturnSignature<TReadModel> method);

    /// <summary>
    /// Add the delete method for resolving the events needed to delete the embedding.
    /// </summary>
    /// <param name="method">The <see cref="DeleteSignature{TReadModel}"/>.</param>
    /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> ResolveDeletionToEvents(DeleteSignature<TReadModel> method);

    /// <summary>
    /// Add the delete method for resolving the events needed to delete the embedding.
    /// </summary>
    /// <param name="method">The <see cref="DeleteSignature{TReadModel}"/>.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> ResolveDeletionToEvents(DeleteEnumerableReturnSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="method">The <see cref="TaskOnSignature{TReadModel, TEvent}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On<TEvent>(TaskOnSignature<TReadModel, TEvent> method)
        where TEvent : class;
        
    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="method">The <see cref="SyncOnSignature{T}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On<TEvent>(SyncOnSignature<TReadModel, TEvent> method)
        where TEvent : class;

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On(EventType eventType, TaskOnSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On(EventType eventType, SyncOnSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, TaskOnSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, SyncOnSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, TaskOnSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, SyncOnSignature<TReadModel> method);
}
