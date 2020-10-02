// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the result from a partitioned filter.
    /// </summary>
    public class PartitionedFilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionedFilterResult"/> class.
        /// </summary>
        /// <param name="shouldInclude">Whether or not the event should be included.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public PartitionedFilterResult(bool shouldInclude, PartitionId partitionId)
        {
            PartitionId = partitionId;
            ShouldInclude = shouldInclude;
        }

        /// <summary>
        /// Gets the <see cref="PartitionId" />.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets a value indicating whether the event should be included or not.
        /// </summary>
        public bool ShouldInclude { get; }
    }
}