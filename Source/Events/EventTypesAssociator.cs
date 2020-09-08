// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an <see cref="ArtifactsAssociatorFor{TArtifact, TArtifactId}" /> that can build <see cref="EventType" /> artifacts.
    /// </summary>
    public class EventTypesAssociator : ArtifactsAssociatorFor<EventType, EventTypeId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesAssociator"/> class.
        /// </summary>
        /// <param name="artifacts">The <see cref="IArtifacts" />.</param>
        public EventTypesAssociator(IArtifacts artifacts)
            : base(artifacts)
        {
        }

        /// <inheritdoc/>
        public override ArtifactsAssociatorFor<EventType, EventTypeId> Associate(Type type, EventTypeId artifactId, Generation generation)
            => Associate(type, new EventType(artifactId, generation));
    }
}
