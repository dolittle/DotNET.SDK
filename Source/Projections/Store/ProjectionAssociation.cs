// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Represents a projection that is associated with a <see cref="Type" />.
    /// </summary>
    public class ProjectionAssociation : Value<ProjectionAssociation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionAssociation"/> class.
        /// </summary>
        /// <param name="identifier">The <see cref="ProjectionId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        public ProjectionAssociation(ProjectionId identifier, ScopeId scopeId)
        {
            Identifier = identifier;
            ScopeId = scopeId;
        }

        /// <summary>
        /// Gets the unique identifier for projection - <see cref="ProjectionId" />.
        /// </summary>
        public ProjectionId Identifier { get; }

        /// <summary>
        /// Gets the scope the projection is in.
        /// </summary>
        public ScopeId ScopeId { get; }
    }
}
