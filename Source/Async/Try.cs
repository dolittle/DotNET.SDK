// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.ExceptionServices;

namespace Dolittle.SDK.Async;

/// <summary>
/// Represents a result that can be successful or unsuccessfull including an <see cref="Exception" />.
/// </summary>
public class Try
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Try"/> class.
    /// </summary>
    public Try() => Success = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Try"/> class.
    /// </summary>
    /// <param name="exception">The optional <see cref="Exception" />.</param>
    public Try(Exception exception) => Exception = exception;

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the <see cref="Exception" /> that caused the operation to fail.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Implicitly convert <see cref="Try" /> to <see cref="Try.Success" />.
    /// </summary>
    /// <param name="try">The <see cref="Try" /> to convert.</param>
    /// <return><see cref="Success" />.</return>
    public static implicit operator bool(Try @try) => @try.Success;

    /// <summary>
    /// Implicitly convert <see cref="Try" /> to <see cref="Try.Exception" />.
    /// </summary>
    /// <param name="try">The <see cref="Try" /> to convert.</param>
    /// <return><see cref="Try.Exception" />.</return>
    public static implicit operator Exception(Try @try) => @try.Exception;

    /// <summary>
    /// Implicitly convert <see cref="bool" /> to <see cref="Try" />.
    /// </summary>
    /// <param name="exception">The <see cref="System.Exception" /> to convert.</param>
    /// <return><see cref="Try" />.</return>
    public static implicit operator Try(Exception exception) => new(exception);

    /// <summary>
    /// Throws the <see cref="Exception"/> with its original stack trace information if this <see cref="Try"/> failed.
    /// </summary>
    public void ThrowIfFailed()
    {
        if (Exception is not null)
        {
            ExceptionDispatchInfo.Capture(Exception).Throw();
        }
    }
}
