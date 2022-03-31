// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// The exception that gets thrown when the <see cref="Harness"/> is not able to start a Runtime container.
/// </summary>
public class FailedToStartRuntime : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToStartRuntime"/> class.
    /// </summary>
    /// <param name="reason">The reason why the Runtime container could not be started.</param>
    public FailedToStartRuntime(string reason)
        : base($"Could not start a Runtime container. {reason}")
    {
    }
}
