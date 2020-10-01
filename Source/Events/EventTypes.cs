// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventTypes" />.
    /// </summary>
    public class EventTypes : Artifacts<EventType, EventTypeId>, IEventTypes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypes"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventTypes(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        protected override Exception CreateNoArtifactAssociatedWithType(Type type)
            => new NoEventTypeAssociatedWithType(type);

        /// <inheritdoc/>
        protected override Exception CreateNoTypeAssociatedWithArtifact(EventType artifact)
            => new NoTypeAssociatedWithEventType(artifact);

        /// <inheritdoc/>
        protected override Exception CreateCannotAssociateMultipleArtifactsWithType(Type type, EventType artifact, EventType existing)
            => new CannotAssociateMultipleEventTypesWithType(type, artifact, existing);

        /// <inheritdoc/>
        protected override Exception CreateCannotAssociateMultipleTypesWithArtifact(EventType artifact, Type type, Type existing)
            => new CannotAssociateMultipleTypesWithEventType(artifact, type, existing);
    }
}
