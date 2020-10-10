// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct <see cref="UncommittedAggregateEvents"/> without an expected <see cref="AggregateRootVersion"/>.
    /// </summary>
    public class ExpectedAggregateRootVersionCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedAggregateRootVersionCannotBeNull"/> class.
        /// </summary>
        public ExpectedAggregateRootVersionCannotBeNull()
            : base($"The expected {nameof(AggregateRootVersion)} of an {nameof(UncommittedAggregateEvents)} cannot be null")
        {
        }
    }
}