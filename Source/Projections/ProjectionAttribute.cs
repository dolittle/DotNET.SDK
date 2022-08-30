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
    readonly ProjectionId _id;
    readonly ScopeId _scope;
    readonly IdentifierAlias _alias; 
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionAttribute"/> class.
    /// </summary>
    /// <param name="projectionId">The unique identifier of the event handler.</param>
    /// <param name="inScope">The scope that the event handler handles events in.</param>
    /// <param name="alias">The alias for the projection.</param>
    public ProjectionAttribute(string projectionId, string? inScope = null, string? alias = null)
    {
        _id = projectionId;
        _scope = inScope ?? ScopeId.Default;
        _alias = alias ?? "";
    }

    /// <inheritdoc />
    public ProjectionModelId GetIdentifier(Type decoratedType) => new(_id, _scope, _alias.Exists ? _alias : decoratedType.Name);
}
