// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Async;
#pragma warning disable
/// <summary>
/// Represents a result that can be successful including a <typeparamref name="TResult">result</typeparamref> or unsuccessfull including an <see cref="Exception" />.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public class Try<TResult>
{
#pragma warning restore
    /// <summary>
    /// Initializes a new instance of the <see cref="Try{TResult}"/> class.
    /// </summary>
    /// <param name="result">The result.</param>
    public Try(TResult result)
    {
        Result = result;
        Success = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Try{TResult}"/> class.
    /// </summary>
    /// <param name="exception">The optional <see cref="Exception" />.</param>
    public Try(Exception exception) => Exception = exception;

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the <typeparamref name="TResult">result</typeparamref>.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Gets the <see cref="Exception" /> that caused the operation to fail.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Implicitly convert <see cref="Try{TResult}" /> to <see cref="Try{TResult}.Success" />.
    /// </summary>
    /// <param name="try">The <see cref="Try{TResult}" /> to convert.</param>
    /// <return><see cref="Try{TResult}.Success" />.</return>
    public static implicit operator bool(Try<TResult> @try) => @try.Success;

    /// <summary>
    /// Implicitly convert <see cref="Try{TResult}" /> to <see cref="Try{TResult}.Result" />.
    /// </summary>
    /// <param name="try">The <see cref="Try{TResult}" /> to convert.</param>
    /// <return><see cref="Try{TResult}.Result" />.</return>
    public static implicit operator TResult(Try<TResult> @try) => @try.Result;

    /// <summary>
    /// Implicitly convert <see cref="Try{TResult}" /> to <see cref="Try{TResult}.Exception" />.
    /// </summary>
    /// <param name="try">The <see cref="Try{TResult}" /> to convert.</param>
    /// <return><see cref="Try{TResult}.Exception" />.</return>
    public static implicit operator Exception(Try<TResult> @try) => @try.Exception;

    /// <summary>
    /// Implicitly convert <typeparamref name="TResult">result</typeparamref> to <see cref="Try{TResult}" />.
    /// </summary>
    /// <param name="result">The <typeparamref name="TResult">result</typeparamref> to convert.</param>
    /// <return><see cref="Try{TResult}" />.</return>
    public static implicit operator Try<TResult>(TResult result) => new(result);

    /// <summary>
    /// Implicitly convert <see cref="bool" /> to <see cref="Try{TResult}" />.
    /// </summary>
    /// <param name="exception">The <see cref="System.Exception" /> to convert.</param>
    /// <return><see cref="Try{TResult}" />.</return>
    public static implicit operator Try<TResult>(Exception exception) => new(exception);
}
