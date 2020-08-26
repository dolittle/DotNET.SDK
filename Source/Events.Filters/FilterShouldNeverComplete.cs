// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Exception that gets thrown when an event filter completes processing.
    /// </summary>
    public class FilterShouldNeverComplete : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterShouldNeverComplete"/> class.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the filter.</param>
        public FilterShouldNeverComplete(FilterId id)
            : base($"Filter {id} completed its processing. Filters should run indefinitely")
        {
        }
    }
}