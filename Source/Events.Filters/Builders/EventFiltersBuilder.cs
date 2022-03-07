// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
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
        var builder = new PrivateEventFilterBuilder(filterId, _modelBuilder);
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
    /// <param name="model">The <see cref="IModel"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public static IUnregisteredEventFilters Build(IModel model, IClientBuildResults buildResults)
    {
        var filters = new UniqueBindings<FilterModelId, ICanRegisterEventFilterProcessor>();
        foreach (var builder in model.GetProcessorBuilderBindings<ICanTryBuildFilter>().Select(_ => _.ProcessorBuilder))
        {
            if (builder.TryBuild(buildResults, out var filter))
            {
                filters.Add(filter.Identifier, filter);
            }
        }
        return new UnregisteredEventFilters(filters);
    }
}
