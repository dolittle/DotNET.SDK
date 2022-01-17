// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events.Handling;

/// <summary>
/// Exception that gets thrown when the runtime could not register an event handler.
/// </summary>
public class FailedToRegisterEventHandler : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToRegisterEventHandler"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public FailedToRegisterEventHandler(string reason)
        : base(reason)
    {
    }
}