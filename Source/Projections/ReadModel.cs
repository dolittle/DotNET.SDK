// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the base class for a read model.
/// </summary>
public abstract class ReadModel
{
#pragma warning disable IDE0032
    DateTimeOffset _lastUpdated;
    ulong _eventOffset;

    /// <summary>
    /// Gets or sets the unique identifier for the read model entity.
    /// </summary>
    public string Id { get; init; } = default!;

    /// <summary>
    /// Get the last time of update for the read model. Reflects when the event was produced, not when the read model was updated.
    /// </summary>
    public DateTimeOffset LastUpdated
    {
        get => _lastUpdated;
        init => _lastUpdated = value;
    }

    public ulong EventOffset
    {
        get => _eventOffset;
        init => _eventOffset = value;
    }
#pragma warning restore IDE0032

    /// <summary>
    /// Internal setter to avoid unintentional overwrite in On-methods.
    /// </summary>
    /// <param name="eventLogSequence">Which event updated the state</param>
    /// <param name="lastUpdated">When the last event that updated the read model was committed</param>
    internal void SetLastUpdated(ulong eventLogSequence, DateTimeOffset lastUpdated)
    {
        _eventOffset = eventLogSequence;
        _lastUpdated = lastUpdated;
    }
}
