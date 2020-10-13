// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents the identification of an event source.
    /// </summary>
    public class EventSourceId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to an <see cref="EventSourceId"/>.
        /// </summary>
        /// <param name="eventSourceId">EventSourceId as <see cref="Guid"/>.</param>
        public static implicit operator EventSourceId(Guid eventSourceId) => new EventSourceId { Value = eventSourceId };

        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="EventSourceId"/>.
        /// </summary>
        /// <param name="eventSourceId">EventSourceId as <see cref="string"/>.</param>
        public static implicit operator EventSourceId(string eventSourceId) => new EventSourceId { Value = Guid.Parse(eventSourceId) };

        /// <summary>
        /// Creates a new instance of <see cref="EventSourceId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="EventSourceId"/>.</returns>
        public static EventSourceId New() => new EventSourceId { Value = Guid.NewGuid() };
    }
}
