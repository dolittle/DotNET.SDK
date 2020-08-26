// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Represents the result of a <see cref="ICanFilterEventsWithPartition"/>.
    /// </summary>
    public class PartitionedFilterResult : FilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionedFilterResult"/> class.
        /// </summary>
        /// <param name="included">true if the <see cref="IEvent"/> should be included in the stream, false if not.</param>
        /// <param name="partition">The <see cref="PartitionId"/> of which the <see cref="IEvent"/> should be put in the stream.</param>
        public PartitionedFilterResult(bool included, PartitionId partition)
            : base(included)
        {
            Partition = partition;
        }

        /// <summary>
        /// Gets the <see cref="PartitionId"/> of which the <see cref="CommittedEvent"/> should be put in the stream.
        /// </summary>
        public PartitionId Partition { get; }

        /// <summary>
        /// Implicitly convert from a <see cref="Tuple{T,U}"/> to <see cref="PartitionedFilterResult"/>.
        /// </summary>
        /// <param name="result">A <see cref="Tuple{T,U}"/> containing the result of the filtering operation.</param>
        public static implicit operator PartitionedFilterResult((bool included, PartitionId partition) result) => new PartitionedFilterResult(result.included, result.partition);
    }
}
