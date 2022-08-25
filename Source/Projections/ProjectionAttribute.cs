// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a class to indicate the Projection Id of the Projection class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ProjectionAttribute : Attribute, IDecoratedTypeDecorator<ProjectionModelId>
{
    readonly ProjectionId _projectionId;
    readonly string? _alias;
    readonly ScopeId _scope;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionAttribute"/> class.
    /// </summary>
    /// <param name="projectionId">The unique identifier of the event handler.</param>
    /// <param name="inScope">The scope that the event handler handles events in.</param>
    /// <param name="alias">The alias for the projection.</param>
    public ProjectionAttribute(string projectionId, string? inScope = null, string? alias = null)
    {
        _projectionId = projectionId;
        _scope = inScope ?? ScopeId.Default;
        _alias = alias;
    }

    /// <inheritdoc />
    public ProjectionModelId GetIdentifier(Type decoratedType) => new(_projectionId, _scope, _alias ?? decoratedType.Name);
}
