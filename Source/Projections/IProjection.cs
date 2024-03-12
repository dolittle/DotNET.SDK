// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Dolittle.SDK.Events;

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
    /// Gets the alias of the projection.
    /// </summary>
    ProjectionAlias? Alias { get; }

    /// <summary>
    /// Gets a value indicating whether the projection has an alias or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Alias))]
    bool HasAlias { get; }


    /// <summary>
    /// Get the idle unload timeout for the projection.
    /// </summary>
    TimeSpan IdleUnloadTimeout { get; }
    
    /// <summary>
    /// Gets a value indicating whether the projection supports historical queries or not.
    /// Only projections that limits the key selectors to eventSourceId support historical queries.
    /// </summary>
    bool SupportsHistoricalQueries { get; }

    /// <summary>
    /// Gets the event types identified by its artifact that is handled by this event handler.
    /// </summary>
    IImmutableDictionary<EventType, KeySelector> Events { get; }
}
