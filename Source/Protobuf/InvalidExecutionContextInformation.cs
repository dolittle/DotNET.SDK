// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Protobuf;

/// <summary>
/// Exception that gets thrown when there is invalid information on an execution context.
/// </summary>
public class InvalidExecutionContextInformation : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidExecutionContextInformation"/> class.
    /// </summary>
    /// <param name="details">The details on what is invalid.</param>
    /// <param name="innerException">The inner exception that caused the failure.</param>
    public InvalidExecutionContextInformation(string details, Exception innerException)
        : base($"Execution context contains invalid information about its {details}", innerException)
    {
    }
}
