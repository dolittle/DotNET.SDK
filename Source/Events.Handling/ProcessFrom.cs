// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// This is used when the event handler does not have any state, and resets the position to the given <see cref="StartFrom" />.
/// </summary>
public enum ProcessFrom
{
    /// <summary>
    ///   Start from the beginning of the stream, process all events.
    /// </summary>
    Earliest,
    /// <summary>
    /// Start after the latest event in the stream, only process new events.
    /// </summary>
    Latest
}
