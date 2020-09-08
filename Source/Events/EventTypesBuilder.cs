// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an <see cref="ArtifactsBuilder{TArtifact, TArtifactId}" /> that can build <see cref="EventType" /> artifacts.
    /// </summary>
    public class EventTypesBuilder : ArtifactsBuilder<EventType, EventTypeId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventTypesBuilder(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <inheritdoc/>
        public override ArtifactsBuilder<EventType, EventTypeId> Associate(Type type, EventTypeId artifactId, Generation generation)
            => Associate(type, new EventType(artifactId, generation));
    }
}
