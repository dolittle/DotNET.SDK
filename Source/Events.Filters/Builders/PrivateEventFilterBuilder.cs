// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events.Filters.Builders.Partitioned;
using Dolittle.SDK.Events.Filters.Builders.Unpartitioned;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Represents the builder for building private event filters.
/// </summary>
public class PrivateEventFilterBuilder : IPrivateEventFilterBuilder
{
    ICanBuildPrivateFilter _filterBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateEventFilterBuilder"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    public PrivateEventFilterBuilder(FilterId filterId) => FilterId = filterId;

    /// <summary>
    /// Gets the <see cref="FilterId" /> of the filter that this builder builds.
    /// </summary>
    public FilterId FilterId { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId" /> the filter operates on.
    /// </summary>
    public ScopeId ScopeId { get; private set; } = ScopeId.Default;

    /// <inheritdoc />
    public IPrivateEventFilterBuilder InScope(ScopeId scopeId)
    {
        ScopeId = scopeId;
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

    /// <summary>
    /// Builds the private filter.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="filter">The outputted <see cref="ICanRegisterEventFilterProcessor"/> that can register the private filter.</param>
    /// <returns>A value indicating whether the building succeeded or not.</returns>
    public bool TryBuild(IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_filterBuilder != default)
        {
            return _filterBuilder.TryBuild(FilterId, ScopeId, buildResults, out filter);
        }
        buildResults.AddError(new FilterDefinitionIncomplete(FilterId, ScopeId, "Call Partitioned() or Handle(...) before building private filter"));
        return false;

    }
}
