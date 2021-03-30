// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Decorates a class to indicate the Projection Id of the Projection class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ProjectionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionAttribute"/> class.
        /// </summary>
        /// <param name="projectionId">The unique identifier of the event handler.</param>
        /// <param name="inScope">The scope that the event handler handles events in.</param>
        public ProjectionAttribute(string projectionId, string inScope = default)
        {
            Identifier = Guid.Parse(projectionId);
            Scope = inScope == default ? ScopeId.Default : inScope;
        }

        /// <summary>
        /// Gets the unique identifier for this projection.
        /// </summary>
        public ProjectionId Identifier { get; }

        /// <summary>
        /// Gets the <see cref="ScopeId" />.
        /// </summary>
        public ScopeId Scope { get; }
    }
}
