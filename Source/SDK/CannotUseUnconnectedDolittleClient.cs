// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK;

/// <summary>
/// Exception that gets thrown when the trying to access properties on the <see cref="IDolittleClient"/> that requires it to be started.
/// </summary>
public class CannotUseUnconnectedDolittleClient : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotUseUnconnectedDolittleClient"/> class.
    /// </summary>
    public CannotUseUnconnectedDolittleClient()
        : base($"{nameof(DolittleClient)} has not been connected yet. Connect the Dolittle Client to the Runtime by calling the {nameof(IDolittleClient.Connect)} method")
    {
    }
}