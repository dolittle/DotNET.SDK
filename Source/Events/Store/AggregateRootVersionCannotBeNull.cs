// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="CommittedAggregateEvent"/> without an <see cref="AggregateRootVersion"/>.
    /// </summary>
    public class AggregateRootVersionCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootVersionCannotBeNull"/> class.
        /// </summary>
        public AggregateRootVersionCannotBeNull()
            : base($"The {nameof(AggregateRootVersion)} of an {nameof(CommittedAggregateEvent)} cannot be null")
        {
        }
    }
}