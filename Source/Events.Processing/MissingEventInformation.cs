// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when there are details missing on an event.
    /// </summary>
    public class MissingEventInformation : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingEventInformation"/> class.
        /// </summary>
        /// <param name="details">The details on what is missing.</param>
        public MissingEventInformation(string details)
            : base($"Event is missing information about its {details}")
        {
        }
    }
}
