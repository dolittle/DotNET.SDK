// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events.Filters.Builders.Partitioned;
using Dolittle.SDK.Events.Filters.Builders.Partitioned.Public;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Represents an implementation of <see cref="IEventFiltersBuilder"/>.
/// </summary>
public class EventFiltersBuilder : IEventFiltersBuilder
{
    readonly IModelBuilder _modelBuilder;

    /// <summary>
    /// Initializes an instance of the <see cref="EventFiltersBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    public EventFiltersBuilder(IModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }

    /// <inheritdoc />
    public IPrivateEventFilterBuilder CreatePrivate(FilterId filterId)
    {
        var builder = new PrivateEventFilterBuilder(new FilterModelId(filterId, ScopeId.Default, ""), _modelBuilder);
        return builder;
    }

    /// <inheritdoc />
    public IPartitionedEventFilterBuilder CreatePublic(FilterId filterId)
    {
        var builder = new PublicEventFilterBuilder(filterId, _modelBuilder);
        return builder;
    }

    /// <summary>
    /// Builds all the event filters.
    /// </summary>
    /// <param name="applicationModel">The <see cref="IApplicationModel"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public static IUnregisteredEventFilters Build(IApplicationModel applicationModel, IClientBuildResults buildResults)
    {
        var filters = new UniqueBindings<FilterModelId, ICanRegisterEventFilterProcessor>();
        foreach (var (identifier, builder) in applicationModel.GetProcessorBuilderBindings<ICanTryBuildFilter, FilterModelId, FilterId>())
        {
            if (builder.TryBuild(identifier, buildResults, out var filter))
            {
                filters.Add(identifier, filter);
            }
        }
        return new UnregisteredEventFilters(filters);
    }
}
