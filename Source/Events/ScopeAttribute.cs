// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events
{
    /// <summary>
    /// Decorates a class to indicate the scope id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScopeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAttribute"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> identifier.</param>
        public ScopeAttribute(string id)
        {
            Id = Guid.Parse(id);
        }

        /// <summary>
        /// Gets the unique id of the scope.
        /// </summary>
        public ScopeId Id { get; }
    }
}