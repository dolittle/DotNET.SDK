// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the identifier of an event handler in an application model.
/// </summary>
public class ProjectionModelId : Identifier<ProjectionId, ScopeId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionModelId"/> class.
    /// </summary>
    /// <param name="id">The <see cref="ProjectionId"/>.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.></param>
    /// <param name="alias">The alias.</param>
    public ProjectionModelId(ProjectionId id, ScopeId scope, IdentifierAlias alias)
        : base("Projection", id, scope, alias)
    {
        Scope = scope;
    }
    
    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    public ScopeId Scope { get; }
}
