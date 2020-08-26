// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Filters.EventHorizon
{
    /// <summary>
    /// Represents the result of a <see cref="ICanFilterPublicEvents"/>.
    /// </summary>
    public class PublicFilterResult : PartitionedFilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterResult"/> class.
        /// </summary>
        /// <param name="included">true if the <see cref="IEvent"/> should be included in the stream, false if not.</param>
        /// <param name="partition">The <see cref="PartitionId"/> of which the <see cref="IEvent"/> should be put in the stream.</param>
        public PublicFilterResult(bool included, PartitionId partition)
            : base(included, partition)
        {
        }

        /// <summary>
        /// Implicitly convert from a <see cref="Tuple{T,U}"/> to <see cref="PublicFilterResult"/>.
        /// </summary>
        /// <param name="result">A <see cref="Tuple{T,U}"/> containing the result of the filtering operation.</param>
        public static implicit operator PublicFilterResult((bool included, PartitionId partition) result) => new PublicFilterResult(result.included, result.partition);
    }
}
