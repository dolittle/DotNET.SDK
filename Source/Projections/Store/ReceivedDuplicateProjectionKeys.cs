// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Exception that gets thrown when getting all projections and there are multiple projection states with the same <see cref="Key"/>.
/// </summary>
public class ReceivedDuplicateProjectionKeys : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="ReceivedDuplicateProjectionKeys"/> class.
    /// </summary>
    /// <param name="projection">The <see cref="ProjectionId"/> that has multiple states with the same <see cref="Key"/>.</param>
    /// <param name="key">The <see cref="Key"/> that occurs multiple times.</param>
    public ReceivedDuplicateProjectionKeys(ProjectionId projection, Key key)
        : base($"For projection {projection} multiple states with the key {key} was received from the Runtime")
    {
    }
}
