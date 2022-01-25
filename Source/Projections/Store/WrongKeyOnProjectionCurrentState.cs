// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Exception that gets thrown when the stored projection state has a different <see cref="Key"/> than expected.
/// </summary>
public class WrongKeyOnProjectionCurrentState : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="WrongKeyOnProjectionCurrentState"/>.
    /// </summary>
    /// <param name="projectionId">THe <see cref="ProjectionId"/>.</param>
    /// <param name="expectedKey">The expected <see cref="Key"/>.</param>
    /// <param name="stateKey">The <see cref="Key"/> on the <see cref="CurrentState{TProjection}"/>.</param>
    public WrongKeyOnProjectionCurrentState(ProjectionId projectionId, Key expectedKey, Key stateKey)
        : base($"Expected key of stored projection state with id {projectionId} to have key {expectedKey} but it is {stateKey}")
    {
    }
}
