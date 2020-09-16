// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when constructing an <see cref="EventIdentifier"/> from an invalid <see cref="string"/>.
    /// </summary>
    public class EventIdentifierStringIsInvalid : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventIdentifierStringIsInvalid"/> class.
        /// </summary>
        /// <param name="eventIdentifier">The invalid event identifier <see cref="string"/>.</param>
        public EventIdentifierStringIsInvalid(string eventIdentifier)
            : base($"The event identifier string \"{eventIdentifier}\" is invalid. It needs to be a Base64 encoded value of 40 bytes.")
        {
        }
    }
}