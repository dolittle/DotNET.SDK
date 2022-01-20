// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events.Handling;

/// <summary>
/// Exception that gets thrown when trying to register an event handler on a read only stream.
/// </summary>
public class CannotRegisterEventHandlerOnNonWriteableStream : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotRegisterEventHandlerOnNonWriteableStream"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public CannotRegisterEventHandlerOnNonWriteableStream(string reason)
        : base(reason)
    {
    }
}