// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// The exception that gets thrown when the <see cref="Harness"/> fails to start a Docker container.
/// </summary>
public class CouldNotStartDockerContainer : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotStartDockerContainer"/> class.
    /// </summary>
    /// <param name="description">A description of what happened.</param>
    public CouldNotStartDockerContainer(string description)
        : base($"Failed to start the Docker container. {description}")
    {
    }
}
