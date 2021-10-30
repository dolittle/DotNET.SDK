// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents the alias of an event type.
    /// </summary>
    public class EventTypeAlias : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="EventTypeAlias"/>.
        /// </summary>
        /// <param name="alias">EventTypeAlias as <see cref="string"/>.</param>
        public static implicit operator EventTypeAlias(string alias) => new EventTypeAlias { Value = alias };
    }
}