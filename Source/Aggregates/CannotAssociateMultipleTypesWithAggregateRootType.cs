// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when attempting to associate multiple instance of <see cref="Type"/> with a single <see cref="AggregateRootType"/>.
    /// </summary>
    public class CannotAssociateMultipleTypesWithAggregateRootType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAssociateMultipleTypesWithAggregateRootType"/> class.
        /// </summary>
        /// <param name="aggregateRootType">The <see cref="AggregateRootType"/> that was attempted to associate with a <see cref="Type"/>.</param>
        /// <param name="type">The <see cref="Type"/> that was attempted to associate with.</param>
        /// <param name="existing">The <see cref="Type"/> that the <see cref="AggregateRootType"/> was already associated with.</param>
        public CannotAssociateMultipleTypesWithAggregateRootType(AggregateRootType aggregateRootType, Type type, Type existing)
            : base($"{aggregateRootType} cannot be associated with {type} because it is already associated with {existing}")
        {
        }
    }
}
