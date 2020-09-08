// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Defines an interface for working directly with the Event Store.
    /// </summary>
    public class EventType : Artifact
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventType"/> class.
        /// </summary>
        /// <param name="id">The <see cref="EventTypeId">unique identifer</see> of the <see cref="EventType"/>.</param>
        public EventType(EventTypeId id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventType"/> class.
        /// </summary>
        /// <param name="id">The <see cref="EventTypeId">unique identifer</see> of the <see cref="EventType"/>.</param>
        /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="Artifact"/>.</param>
        public EventType(ArtifactId id, Generation generation)
            : base(id, generation)
        {
        }
    }
}
