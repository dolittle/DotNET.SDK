// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
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
    ScopeId _scopeId = ScopeId.Default;
    ICanBuildPrivateFilter _filterBuilder;

    FilterModelId ModelId => new(_filterId, _scopeId, null);

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateEventFilterBuilder"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="_filterId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    public PrivateEventFilterBuilder(FilterId filterId, IModelBuilder modelBuilder)
    {
        _filterId = filterId;
        _modelBuilder = modelBuilder;
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildFilter>(ModelId, this);
    }
    
    /// <inheritdoc />
    public bool Equals(ICanTryBuildFilter other) => ReferenceEquals(this, other);

    /// <inheritdoc />
    public IPrivateEventFilterBuilder InScope(ScopeId scopeId)
    {
        _modelBuilder.UnbindIdentifierToProcessorBuilder<ICanTryBuildFilter>(ModelId, this);
        _scopeId = scopeId;
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildFilter>(ModelId, this);
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
    public bool TryBuild(FilterModelId identifier, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_filterBuilder != default)
        {
            return _filterBuilder.TryBuild(identifier, _scopeId, buildResults, out filter);
        }
        buildResults.AddError(identifier, new FilterDefinitionIncomplete(_filterId, _scopeId, "Call Partitioned() or Handle(...) before building private filter"));
        return false;
    }
}
