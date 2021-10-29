// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when attempting to associate multiple instance of <see cref="AggregateRootType"/> with a single <see cref="Type"/>.
    /// </summary>
    public class CannotAssociateMultipleAggregateRootTypesWithType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAssociateMultipleAggregateRootTypesWithType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that was attempted to associate with a <see cref="AggregateRootType"/>.</param>
        /// <param name="aggregateRootType">The <see cref="AggregateRootType"/> that was attempted to associate with.</param>
        /// <param name="existing">The <see cref="AggregateRootType"/> that the <see cref="Type"/> was already associated with.</param>
        public CannotAssociateMultipleAggregateRootTypesWithType(Type type, AggregateRootType aggregateRootType, AggregateRootType existing)
            : base($"{type} cannot be associated with {aggregateRootType} because it is already associated with {existing}")
        {
        }
    }
}
