// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Decorates a method to indicate the <see cref="FilterId" /> of the Filter class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterAttribute"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> identifier.</param>
        public FilterAttribute(string id)
        {
            Id = Guid.Parse(id);
        }

        /// <summary>
        /// Gets the unique id for this event processor.
        /// </summary>
        public FilterId Id { get; }
    }
}