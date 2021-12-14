// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned;

/// <summary>
/// Defines a builder for building partitioned event filters.
/// </summary>
public interface IPartitionedEventFilterBuilder
{
    /// <summary>
    /// Defines a callback for the filter.
    /// </summary>
    /// <param name="callback">The callback that will be called for each event.</param>
    void Handle(PartitionedFilterEventCallback callback);
}
