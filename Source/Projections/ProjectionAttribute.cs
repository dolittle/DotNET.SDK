// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a class to indicate the Projection Id of the Projection class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ProjectionAttribute : Attribute, IDecoratedTypeDecorator<ProjectionModelId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionAttribute"/> class.
    /// </summary>
    /// <param name="projectionId">The unique identifier of the event handler.</param>
    /// <param name="inScope">The scope that the event handler handles events in.</param>
    /// <param name="alias">The alias for the projection.</param>
    public ProjectionAttribute(string projectionId, string inScope = default, string alias = default)
    {
        Identifier = Guid.Parse(projectionId);
        Scope = inScope ?? ScopeId.Default;
        if (alias != default)
        {
            Alias = alias;
            HasAlias = true;
        }
    }

    /// <summary>
    /// Gets the unique identifier for this projection.
    /// </summary>
    public ProjectionId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId" />.
    /// </summary>
    public ScopeId Scope { get; }

    /// <summary>
    /// Gets the <see cref="ProjectionAlias"/>.
    /// </summary>
    public ProjectionAlias Alias { get; }

    /// <summary>
    /// Gets a value indicating whether this event handler has an alias.
    /// </summary>
    public bool HasAlias { get; }

    /// <inheritdoc />
    public ProjectionModelId GetIdentifier() => new(Identifier, Scope);
}
