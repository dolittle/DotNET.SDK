// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Defines a builder for building a projection for a read model from method callbacks.
/// </summary>
/// <typeparam name="TReadModel">The type of the projection read model.</typeparam>
public interface IProjectionBuilderForReadModel<TReadModel>
    where TReadModel : class, new()
{
    /// <summary>
    /// Defines the projection to operate in a specific <see cref="ScopeId" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilderForReadModel<TReadModel> InScope(ScopeId scopeId);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
    /// <param name="method">The <see cref="SyncProjectionSignature{T}" />.</param>
    /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    IProjectionBuilderForReadModel<TReadModel> On<TEvent>(KeySelectorSignature<TEvent> selectorCallback, SyncProjectionSignature<TReadModel, TEvent> method)
        where TEvent : class;

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
    /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
    /// <param name="method">The <see cref="SyncProjectionSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilderForReadModel<TReadModel> On(EventType eventType, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
    /// <param name="method">The <see cref="SyncProjectionSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method);

    /// <summary>
    /// Add a method for updating a projection on an event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
    /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
    /// <param name="method">The <see cref="SyncProjectionSignature{TReadModel}" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method);

    /// <summary>
    /// Defines the projection to have a specific <see cref="ProjectionAlias" />.
    /// </summary>
    /// <param name="alias">The <see cref="ProjectionAlias" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilderForReadModel<TReadModel> WithAlias(ProjectionAlias alias);
}
