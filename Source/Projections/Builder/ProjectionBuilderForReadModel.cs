// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents a builder for building projection on-methods.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
public class ProjectionBuilderForReadModel<TReadModel> : IProjectionBuilderForReadModel<TReadModel>, ICanTryBuildProjection
    where TReadModel : ReadModel, new()
{
    readonly IList<IProjectionMethod<TReadModel>> _methods = new List<IProjectionMethod<TReadModel>>();
    readonly ProjectionId _projectionId;
    ProjectionAlias _alias;
    ScopeId _scopeId;
    readonly IModelBuilder _modelBuilder;
    readonly ProjectionBuilder _parentBuilder;

    ProjectionModelId ModelId => new(_projectionId, _scopeId, _alias);

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
        ProjectionBuilder parentBuilder)
    {
        _projectionId = projectionId;
        _alias = typeof(TReadModel).Name;
        _scopeId = scopeId;
        _modelBuilder = modelBuilder;
        _parentBuilder = parentBuilder;
        BindModel();
    }
    
    /// <inheritdoc />
    public bool Equals(ICanTryBuildProjection? other) => ReferenceEquals(this, other);

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> InScope(ScopeId scopeId)
    {
        UnbindModel();
        _scopeId = scopeId;
        BindModel();
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On<TEvent>(KeySelectorSignature<TEvent> selectorCallback, ProjectionSignature<TReadModel, TEvent> method)
        where TEvent : class
    {
        _methods.Add(new TypedProjectionMethod<TReadModel, TEvent>(method, selectorCallback(new KeySelectorBuilder<TEvent>())));
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventType eventType, KeySelectorSignature selectorCallback, ProjectionSignature<TReadModel> method)
    {
        _methods.Add(new ProjectionMethod<TReadModel>(method, selectorCallback(new KeySelectorBuilder()), eventType));
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, KeySelectorSignature selectorCallback, ProjectionSignature<TReadModel> method)
        => On(new EventType(eventTypeId), selectorCallback, method);


    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, KeySelectorSignature selectorCallback, ProjectionSignature<TReadModel> method)
        => On(new EventType(eventTypeId, eventTypeGeneration), selectorCallback, method);

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> WithAlias(ProjectionAlias alias)
    {
        UnbindModel();
        _alias = alias;
        BindModel();
        return this;
    }
    
    /// <inheritdoc />
    public bool TryBuild(ProjectionModelId identifier, IEventTypes eventTypes, IClientBuildResults buildResults, [NotNullWhen(true)] out IProjection? projection)
    {
        projection = default;
        var eventTypesToMethods = new Dictionary<EventType, IProjectionMethod<TReadModel>>();
        if (!TryAddOnMethods(identifier, eventTypes, eventTypesToMethods, buildResults))
        {
            buildResults.AddFailure(identifier, "One or more projection methods could not be built", "Maybe it tries to handle the same type of event twice?");
            return false;
        }

        if (eventTypesToMethods.Any())
        {
            projection = new Projection<TReadModel>(identifier, eventTypesToMethods);
            return true;
        }
        buildResults.AddFailure(identifier, "No projection methods are configured for projection", "Handle an event by calling one of the On-methods on the projection builder");
        return false;
    }
    
    bool TryAddOnMethods(ProjectionModelId identifier, IEventTypes eventTypes, IDictionary<EventType, IProjectionMethod<TReadModel>> eventTypesToMethods, IClientBuildResults buildResults)
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
            buildResults.AddFailure(identifier, $"Multiple handlers for event type {eventType}");
        }
        return okay;
    }

    void BindModel()
    {
        _modelBuilder.BindIdentifierToType<ProjectionModelId, ProjectionId>(ModelId, typeof(TReadModel));
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildProjection>(ModelId, _parentBuilder);
    }

    void UnbindModel()
    {
        _modelBuilder.UnbindIdentifierToType<ProjectionModelId, ProjectionId>(ModelId, typeof(TReadModel));
        _modelBuilder.UnbindIdentifierToProcessorBuilder<ICanTryBuildProjection>(ModelId, _parentBuilder);
    }
}
