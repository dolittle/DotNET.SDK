// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// The exception that gets thrown when the <see cref="Harness"/> fails to find a Docker container.
/// </summary>
public class CouldNotFindDockerContainer : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotFindDockerContainer"/> class.
    /// </summary>
    /// <param name="containerId">The container ID that was not found.</param>
    public CouldNotFindDockerContainer(string containerId)
        : base($"Could not find a container with id {containerId}")
    {
    }
}
