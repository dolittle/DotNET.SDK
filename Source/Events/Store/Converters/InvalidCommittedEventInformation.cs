// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Exception that gets thrown when there is invalid information on a committed event.
/// </summary>
public class InvalidCommittedEventInformation : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCommittedEventInformation"/> class.
    /// </summary>
    /// <param name="details">The details on what is invalid.</param>
    /// <param name="innerException">The inner exception that caused the failure.</param>
    public InvalidCommittedEventInformation(string details, Exception innerException)
        : base($"Committed event contains invalid information about its {details}", innerException)
    {
    }
}
