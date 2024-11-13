// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.SDK.Services;

/// <summary>
/// Represents an implementation of <see cref="ICoordinateProcessing" />.
/// </summary>
public class ProcessingCoordinator : ICoordinateProcessing
{
    readonly List<Task> _processors = [];

    /// <inheritdoc/>
    public Task Completion => Task.WhenAll(_processors);

    /// <inheritdoc/>
    public void RegisterProcessor(Task processor) => _processors.Add(processor);
}
