// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events.Filters.Builders.Partitioned;
using Dolittle.SDK.Events.Filters.Builders.Partitioned.Public;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Represents an implementation of <see cref="IEventFiltersBuilder"/>.
/// </summary>
public class EventFiltersBuilder : IEventFiltersBuilder
{
    readonly List<PrivateEventFilterBuilder> _privateFilterBuilders = new();
    readonly List<PublicEventFilterBuilder> _publicFilterBuilders = new();
    
    /// <inheritdoc />
    public IEventFiltersBuilder CreatePrivateFilter(FilterId filterId, Action<IPrivateEventFilterBuilder> callback)
    {
        var builder = new PrivateEventFilterBuilder(filterId);
        callback(builder);
        _privateFilterBuilders.Add(builder);
        return this;
    }

    /// <inheritdoc />
    public IEventFiltersBuilder CreatePublicFilter(FilterId filterId, Action<IPartitionedEventFilterBuilder> callback)
    {
        var builder = new PublicEventFilterBuilder(filterId);
        callback(builder);
        _publicFilterBuilders.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds all the event filters.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public IUnregisteredEventFilters Build(IClientBuildResults buildResults)
    {
        var filters = new List<ICanRegisterEventFilterProcessor>();
        foreach (var builder in _privateFilterBuilders)
        {
            if (builder.TryBuild(buildResults, out var filter))
            {
                filters.Add(filter);
            }
        }
        foreach (var builder in _publicFilterBuilders)
        {
            if (builder.TryBuild(buildResults, out var filter))
            {
                filters.Add(filter);
            }
        }
        return new UnregisteredEventFilters(filters);
    }
}
