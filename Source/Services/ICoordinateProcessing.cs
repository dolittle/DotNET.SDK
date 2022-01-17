// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a system that coordinates processing.
/// </summary>
public interface ICoordinateProcessing
{
    /// <summary>
    /// Gets a <see cref="Task"/> that represents the completion of all of the processing.
    /// </summary>
    Task Completion { get; }

    /// <summary>
    /// Register a processor to wait for.
    /// </summary>
    /// <param name="processor">The <see cref="Task"/> that represents processor to wait for.</param>
    void RegisterProcessor(Task processor);
}