// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Determine what date/time range of events to process.
/// </summary>
public record ProcessRange
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Mode">Start from the start of the event log or just process new events</param>
    /// <param name="StartFrom">Do not process events from before this. Can be in the future, Letting an event handler take over at a given point. (Optional)</param>
    /// <param name="StopAt">Do not process events that were committed after this. (Optional)</param>
    public ProcessRange(ProcessFrom Mode = ProcessFrom.Earliest,
        DateTimeOffset? StartFrom = null,
        DateTimeOffset? StopAt = null)
    {
        this.Mode = Mode;
        this.StartFrom = StartFrom;
        this.StopAt = StopAt;
        if (StartFrom.HasValue && StopAt.HasValue && StartFrom.Value > StopAt.Value)
        {
            throw new ArgumentException("StartFrom cannot be after StopAt");
        }
    }

    public ProcessFrom Mode { get; init; }

    /// <summary>Mode: Start from the start of the event log or just process new events</summary>
    public DateTimeOffset? StartFrom { get; init; }

    /// <summary>Do not process events that were committed after this (Optional)</summary>
    public DateTimeOffset? StopAt { get; init; }
}
