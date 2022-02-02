// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Builder.Copies;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents a builder for building projection on-methods.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
public class ProjectionBuilderForReadModel<TReadModel> : IProjectionBuilderForReadModel<TReadModel>, ICanTryBuildProjection
    where TReadModel : class, new()
{
    readonly IList<IProjectionMethod<TReadModel>> _methods = new List<IProjectionMethod<TReadModel>>();
    readonly ProjectionId _projectionId;
    ScopeId _scopeId;
    readonly IModelBuilder _modelBuilder;
    readonly ProjectionBuilder _parentBuilder;
    readonly IProjectionCopyDefinitionBuilder<TReadModel> _projectionCopyDefinitionBuilder;

    // readonly IProjectionCopyToMongoDBBuilder<> _mongoCopyBuilder;

    ProjectionModelId ModelId => new(_projectionId, _scopeId);

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionBuilderForReadModel{TReadModel}"/> class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="parentBuilder">The <see cref="ProjectionBuilder"/>.</param>
    /// <param name="copyDefinitionBuilder">The <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.</param>
    public ProjectionBuilderForReadModel(
        ProjectionId projectionId,
        ScopeId scopeId,
        IModelBuilder modelBuilder,
        ProjectionBuilder parentBuilder,
        IProjectionCopyDefinitionBuilder<TReadModel> copyDefinitionBuilder)
    {
        _projectionId = projectionId;
        _scopeId = scopeId;
        _modelBuilder = modelBuilder;
        _parentBuilder = parentBuilder;
        _projectionCopyDefinitionBuilder = copyDefinitionBuilder;
        
        modelBuilder.BindIdentifierToType<ProjectionModelId, ProjectionId>(ModelId, typeof(TReadModel));
        modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildProjection>(ModelId, _parentBuilder);
    }
    
    /// <inheritdoc />
    public bool Equals(ICanTryBuildProjection other) => ReferenceEquals(this, other);

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> InScope(ScopeId scopeId)
    {
        _modelBuilder.UnbindIdentifierToProcessorBuilder<ICanTryBuildProjection>(ModelId, _parentBuilder);
        _scopeId = scopeId;
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildProjection>(ModelId, _parentBuilder);
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On<TEvent>(KeySelectorSignature<TEvent> selectorCallback, TaskProjectionSignature<TReadModel, TEvent> method)
        where TEvent : class
    {
        _methods.Add(new TypedProjectionMethod<TReadModel, TEvent>(method, selectorCallback(new KeySelectorBuilder<TEvent>())));
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On<TEvent>(KeySelectorSignature<TEvent> selectorCallback, SyncProjectionSignature<TReadModel, TEvent> method)
        where TEvent : class
    {
        _methods.Add(new TypedProjectionMethod<TReadModel, TEvent>(method, selectorCallback(new KeySelectorBuilder<TEvent>())));
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventType eventType, KeySelectorSignature selectorCallback, TaskProjectionSignature<TReadModel> method)
    {
        _methods.Add(new ProjectionMethod<TReadModel>(method, eventType, selectorCallback(new KeySelectorBuilder())));
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventType eventType, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method)
    {
        _methods.Add(new ProjectionMethod<TReadModel>(method, eventType, selectorCallback(new KeySelectorBuilder())));
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, KeySelectorSignature selectorCallback, TaskProjectionSignature<TReadModel> method)
        => On(new EventType(eventTypeId), selectorCallback, method);

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method)
        => On(new EventType(eventTypeId), selectorCallback, method);


    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, KeySelectorSignature selectorCallback, TaskProjectionSignature<TReadModel> method)
        => On(new EventType(eventTypeId, eventTypeGeneration), selectorCallback, method);

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method)
        => On(new EventType(eventTypeId, eventTypeGeneration), selectorCallback, method);

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> CopyToMongoDB(Action<IProjectionCopyToMongoDBBuilder<TReadModel>> callback = default)
    {
        _projectionCopyDefinitionBuilder.CopyToMongoDB(callback);
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, out IProjection projection)
    {
        projection = default;
        var eventTypesToMethods = new Dictionary<EventType, IProjectionMethod<TReadModel>>();
        if (!TryAddOnMethods(eventTypes, eventTypesToMethods, buildResults))
        {
            buildResults.AddFailure($"Failed to build projection {_projectionId}. One or more projection methods could not be built", "Maybe it tries to handle the same type of event twice?");
            return false;
        }

        if (!_projectionCopyDefinitionBuilder.TryBuild(buildResults, out var projectionCopies))
        {
            buildResults.AddFailure($"Failed to build projection copies definition for projection {_projectionId}");
            return false;
        }

        if (eventTypesToMethods.Any())
        {
            projection = new Projection<TReadModel>(_projectionId, _scopeId, eventTypesToMethods, projectionCopies);
            return true;
        }
        buildResults.AddFailure($"Failed to build projection {_projectionId}. No projection methods are configured for projection", "Handle an event by calling one of the On-methods on the projection builder");
        return false;
    }
    
    bool TryAddOnMethods(IEventTypes eventTypes, IDictionary<EventType, IProjectionMethod<TReadModel>> eventTypesToMethods, IClientBuildResults buildResults)
    {
        var okay = true;
        foreach (var method in _methods)
        {
            var eventType = method.GetEventType(eventTypes);
            if (eventTypesToMethods.TryAdd(eventType, method))
            {
                continue;
            }
            okay = false;
            buildResults.AddFailure($"Projection {_projectionId} already handles event with event type {eventType}");
        }
        return okay;
    }
}
