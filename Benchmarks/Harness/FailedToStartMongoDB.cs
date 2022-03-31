// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// The exception that gets thrown when the <see cref="Harness"/> is not able to start a MongoDB server container.
/// </summary>
public class FailedToStartMongoDB : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToStartMongoDB"/> class.
    /// </summary>
    /// <param name="reason">The reason why the MongoDB container could not be started.</param>
    public FailedToStartMongoDB(string reason)
        : base($"Could not start a MongoDB container. {reason}")
    {
    }
}
