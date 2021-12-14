// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Filters.Builders.Partitioned;
using Dolittle.SDK.Events.Filters.Builders.Unpartitioned;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Defines a builder for building private event filters
/// </summary>
public interface IPrivateEventFilterBuilder
{
    /// <summary>
    /// Defines which <see cref="ScopeId" /> the filter operates on.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <returns>The builder instance.</returns>
    IPrivateEventFilterBuilder InScope(ScopeId scopeId);

    /// <summary>
    /// Defines the filter to be partitioned.
    /// </summary>
    /// <returns>The partitioned event filter builder continuation.</returns>
    IPartitionedEventFilterBuilder Partitioned();

    /// <summary>
    /// Defines the filter to not be partitioned.
    /// </summary>
    /// <returns>The unpartitioned event filter builder continuation.</returns>
    IUnpartitionedEventFilterBuilder Unpartitioned();
}
