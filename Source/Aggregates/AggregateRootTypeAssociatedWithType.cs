// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Type" /> does not have an <see cref="AggregateRootType"/> association.
    /// </summary>
    public class AggregateRootTypeAssociatedWithType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootTypeAssociatedWithType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that has a missing association.</param>
        public AggregateRootTypeAssociatedWithType(Type type)
            : base($"{type} is not associated with an EventType")
        {
        }
    }
}
