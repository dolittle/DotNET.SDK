// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> that can build <see cref="EventType" /> artifacts.
    /// </summary>
    public class EventTypesBuilder : ArtifactsBuilderFor<IEventTypes, EventType, EventTypeId, EventTypesBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventTypesBuilder(ILoggerFactory loggerFactory)
            : base(new EventTypes(loggerFactory.CreateLogger<EventTypes>()))
        {
        }

        /// <inheritdoc/>
        public override EventTypesBuilder Associate(Type type, EventType artifact)
        {
            Artifacts.Associate(type, artifact);
            return this;
        }

        /// <inheritdoc/>
        public override EventTypesBuilder Associate(Type type, EventTypeId artifactId, Generation generation)
            => Associate(type, new EventType(artifactId, generation));
    }
}
