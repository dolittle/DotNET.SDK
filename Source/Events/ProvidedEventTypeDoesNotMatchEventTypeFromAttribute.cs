// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="EventType" /> is being associated to a <see cref="Type" /> that has an <see cref="EventTypeAttribute" /> that specifies another <see cref="EventType"/>.
    /// </summary>
    public class ProvidedEventTypeDoesNotMatchEventTypeFromAttribute : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvidedEventTypeDoesNotMatchEventTypeFromAttribute"/> class.
        /// </summary>
        /// <param name="providedEventType">The <see cref="EventType" /> that the <see cref="Type" /> is being associated with.</param>
        /// <param name="attributeEventType">The <see cref="EventType" /> that is specified in the <see cref="EventTypeAttribute"/>.</param>
        /// <param name="type">The <see cref="Type" /> to associate the <see cref="Type" /> to.</param>
        public ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(EventType providedEventType, EventType attributeEventType, Type type)
            : base($"Attempting to associate {type} with {providedEventType} but it has an [EventType(...)] attribute that specifies {attributeEventType}")
        {
        }
    }
}