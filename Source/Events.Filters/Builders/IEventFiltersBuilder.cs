// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events.Filters.Builders.Partitioned;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Defines the builder for building event filters.
/// </summary>
public interface IEventFiltersBuilder
{
    /// <summary>
    /// Start building for a private filter.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    /// <param name="callback">Callback for building the event filter.</param>
    /// <returns>Continuation of the builder.</returns>
    IEventFiltersBuilder CreatePrivateFilter(FilterId filterId, Action<IPrivateEventFilterBuilder> callback);

    /// <summary>
    /// Start building for a public filter.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    /// <param name="callback">Callback for building the event filter.</param>
    /// <returns>Continuation of the builder.</returns>
    IEventFiltersBuilder CreatePublicFilter(FilterId filterId, Action<IPartitionedEventFilterBuilder> callback);
}
