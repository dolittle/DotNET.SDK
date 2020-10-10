// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct <see cref="UncommittedAggregateEvents"/> without an <see cref="AggregateRootId"/>.
    /// </summary>
    public class AggregateRootIdCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootIdCannotBeNull"/> class.
        /// </summary>
        public AggregateRootIdCannotBeNull()
        : base($"The {nameof(AggregateRootId)} of an {nameof(UncommittedAggregateEvents)} cannot be null")
        {
        }
    }
}