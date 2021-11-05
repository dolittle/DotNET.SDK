// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="AggregateRootType" /> does not have an <see cref="Type"/> association.
    /// </summary>
    public class NoTypeAssociatedWithAggregateRootType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoTypeAssociatedWithAggregateRootType"/> class.
        /// </summary>
        /// <param name="aggregateRootType">The <see cref="AggregateRootType" /> that has a missing association.</param>
        public NoTypeAssociatedWithAggregateRootType(AggregateRootType aggregateRootType)
            : base($"{aggregateRootType} is not associated with a Type")
        {
        }
    }
}
