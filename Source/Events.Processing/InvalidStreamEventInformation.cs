// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events;

/// <summary>
/// Exception that gets thrown when there is invalid information on a stream event.
/// </summary>
public class InvalidStreamEventInformation : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidStreamEventInformation"/> class.
    /// </summary>
    /// <param name="details">The details on what is invalid.</param>
    /// <param name="innerException">The inner exception that caused the failure.</param>
    public InvalidStreamEventInformation(string details, Exception innerException)
        : base($"Stream event contains invalid information about its {details}", innerException)
    {
    }
}