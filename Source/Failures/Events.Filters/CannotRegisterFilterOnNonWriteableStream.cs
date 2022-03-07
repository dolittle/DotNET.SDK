// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events.Filters;

/// <summary>
/// Exception that gets thrown when trying to register a filter on a read only stream.
/// </summary>
public class CannotRegisterFilterOnNonWriteableStream : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotRegisterFilterOnNonWriteableStream"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public CannotRegisterFilterOnNonWriteableStream(string reason)
        : base(reason)
    {
    }
}