// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    /// <returns>Continuation of the builder.</returns>
    IPrivateEventFilterBuilder CreatePrivate(FilterId filterId);

    /// <summary>
    /// Start building for a public filter.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    /// <returns>Continuation of the builder.</returns>
    IPartitionedEventFilterBuilder CreatePublic(FilterId filterId);
}
