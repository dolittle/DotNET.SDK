// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Services;

/// <summary>
/// Exception that gets thrown when <see cref="IAsyncEnumerable{TServerMessage}.GetAsyncEnumerator"/> is called multiple times.
/// </summary>
public class CannotEnumerateServerStreamingCallMultipleTimes : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotEnumerateServerStreamingCallMultipleTimes"/> class.
    /// </summary>
    public CannotEnumerateServerStreamingCallMultipleTimes()
        : base($"Cannot enumerate {typeof(IServerStreamingEnumerable<>).Name} multiple times.")
    {
    }
}
