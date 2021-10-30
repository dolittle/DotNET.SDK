// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="AggregateRootType" /> is being associated to a <see cref="Type" /> that has an <see cref="AggregateRootAttribute" /> that specifies another <see cref="AggregateRootType"/>.
    /// </summary>
    public class ProvidedAggregateRootTypeDoesNotMatchAggregateRootTypeFromAttribute : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvidedAggregateRootTypeDoesNotMatchAggregateRootTypeFromAttribute"/> class.
        /// </summary>
        /// <param name="providedAggregateRoot">The <see cref="AggregateRootType" /> that the <see cref="Type" /> is being associated with.</param>
        /// <param name="attributeAggregateRoot">The <see cref="AggregateRootType" /> that is specified in the <see cref="EventTypeAttribute"/>.</param>
        /// <param name="type">The <see cref="Type" /> to associate the <see cref="Type" /> to.</param>
        public ProvidedAggregateRootTypeDoesNotMatchAggregateRootTypeFromAttribute(AggregateRootType providedAggregateRoot, AggregateRootType attributeAggregateRoot, Type type)
            : base($"Attempting to associate {type} with {providedAggregateRoot} but it has an [AggregateRoot(...)] attribute that specifies {attributeAggregateRoot}")
        {
        }
    }
}