// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services;

/// <summary>
/// Exception that gets thrown when the server does not send a ping within the specified ping interval on an open reverse call.
/// </summary>
public class PingTimedOut : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PingTimedOut"/> class.
    /// </summary>
    /// <param name="timeSpan">The interval which the client asked the server to use between ping messages.</param>
    public PingTimedOut(TimeSpan timeSpan)
        : base($"Timed out while waiting for a ping from the server. Expected a ping every {timeSpan.TotalSeconds} seconds.")
    {
    }
}
