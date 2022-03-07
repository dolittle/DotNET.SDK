// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Defines a projection.
/// </summary>
public interface IProjection
{
    /// <summary>
    /// The <see cref="Type" /> of the projection.
    /// </summary>
    Type ProjectionType { get; }

    /// <summary>
    /// Gets the unique identifier for projection - <see cref="ProjectionId" />.
    /// </summary>
    ProjectionId Identifier { get; }

    /// <summary>
    /// Gets the scope the projection is in.
    /// </summary>
    ScopeId ScopeId { get; }

    /// <summary>
    /// Gets the <see cref="ProjectionCopies"/>.
    /// </summary>
    ProjectionCopies Copies { get; }
    
    /// <summary>
    /// Gets the alias of the projection.
    /// </summary>
    ProjectionAlias Alias { get; }

    /// <summary>
    /// Gets a value indicating whether the projection has an alias or not.
    /// </summary>
    bool HasAlias { get; }

    /// <summary>
    /// Gets the event types identified by its artifact that is handled by this event handler.
    /// </summary>
    IEnumerable<EventSelector> Events { get; }
}
