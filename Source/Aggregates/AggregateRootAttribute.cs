// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// The attribute for deciding the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateRootAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootAttribute"/> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        public AggregateRootAttribute(string id) => Id = id;

        /// <summary>
        /// Gets the <see cref="AggregateRootId" />.
        /// </summary>
        public AggregateRootId Id { get; }
    }
}