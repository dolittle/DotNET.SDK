// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when there is information missing on a committed event.
    /// </summary>
    public class MissingCommittedEventInformation : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingCommittedEventInformation"/> class.
        /// </summary>
        /// <param name="details">The details on what is missing.</param>
        public MissingCommittedEventInformation(string details)
            : base($"Committed event is missing information about its {details}")
        {
        }
    }
}
