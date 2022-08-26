// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events.Filters.Builders.Partitioned;
using Dolittle.SDK.Events.Filters.Builders.Unpartitioned;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Represents the builder for building private event filters.
/// </summary>
public class PrivateEventFilterBuilder : IPrivateEventFilterBuilder, ICanTryBuildFilter
{
    readonly FilterId _filterId;
    readonly IModelBuilder _modelBuilder;
    ScopeId _scopeId;
    IdentifierAlias _alias;
    ICanBuildPrivateFilter? _filterBuilder;

    FilterModelId ModelId => new(_filterId, _scopeId, _alias);

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateEventFilterBuilder"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="_filterId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    public PrivateEventFilterBuilder(FilterModelId filterId, IModelBuilder modelBuilder)
    {
        _filterId = filterId.Id;
        _scopeId = filterId.Scope;
        _alias = filterId.Alias;
        _modelBuilder = modelBuilder;
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildFilter, FilterModelId, FilterId>(ModelId, this);
    }
    
    /// <inheritdoc />
    public bool Equals(IProcessorBuilder<FilterModelId, FilterId> other)
        => other is PrivateEventFilterBuilder && ReferenceEquals(this, other);

    /// <inheritdoc />
    public IPrivateEventFilterBuilder InScope(ScopeId scopeId)
    {
        _modelBuilder.UnbindIdentifierToProcessorBuilder<ICanTryBuildFilter, FilterModelId, FilterId>(ModelId, this);
        _scopeId = scopeId;
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildFilter, FilterModelId, FilterId>(ModelId, this);
        return this;
    }

    /// <inheritdoc />
    public IPartitionedEventFilterBuilder Partitioned()
    {
        var builder = new PartitionedEventFilterBuilder();
        _filterBuilder = builder;
        return builder;
    }

    /// <inheritdoc />
    public IUnpartitionedEventFilterBuilder Unpartitioned()
    {
        var builder = new UnpartitionedEventFilterBuilder();
        _filterBuilder = builder;
        return builder;
    }
    
    /// <inheritdoc />
    public bool TryBuild(FilterModelId filterId, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_filterBuilder != default)
        {
            return _filterBuilder.TryBuild(filterId, buildResults, out filter);
        }
        buildResults.AddError(new FilterDefinitionIncomplete(_filterId, _scopeId, "Call Partitioned() or Handle(...) before building private filter"));
        return false;
    }
}
