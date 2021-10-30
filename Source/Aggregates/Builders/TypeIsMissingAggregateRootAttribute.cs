// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates.Builders
{
    /// <summary>
    /// Exception that gets thrown when an event type <see cref="Type" /> is missing an <see cref="AggregateRootAttribute" />.
    /// </summary>
    public class TypeIsMissingAggregateRootAttribute : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeIsMissingAggregateRootAttribute"/> class.
        /// </summary>
        /// <param name="type">The event type <see cref="Type" />.</param>
        public TypeIsMissingAggregateRootAttribute(Type type)
            : base($"{type} is missing the [AggregateRoot(...)] attribute")
        {
        }
    }
}