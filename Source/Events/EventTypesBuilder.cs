// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> that can build <see cref="EventType" /> artifacts.
    /// </summary>
    public class EventTypesBuilder : ArtifactsBuilderFor<IEventTypes, EventType, EventTypeId, EventTypesBuilder>
    {
        /// <summary>
        /// Associate <typeparamref name="TEventType"/> with the <see cref="EventType" /> that it is decorated with.
        /// </summary>
        /// <typeparam name="TEventType">The <see cref="Type" /> of the event type class.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder" /> for continuation.</returns>
        public EventTypesBuilder Associate<TEventType>()
            where TEventType : class
        {
            var type = typeof(TEventType);
            if (!TryGetDecoratedEventType(type, out var eventType)) throw new EventTypeNotDecorated(type);

            return Associate(type, eventType);
        }

        /// <inheritdoc/>
        public override EventTypesBuilder Associate(Type type, EventType eventType)
        {
            if (TryGetDecoratedEventType(type, out var decoratedEventType)
                && decoratedEventType != eventType)
            {
                throw new EventTypeDoesNotMatchDecoratedEventType(eventType, decoratedEventType, type);
            }

            Associations.Add((type, eventType));
            return this;
        }

        /// <inheritdoc/>
        public override EventTypesBuilder Associate(Type type, EventTypeId artifactId, Generation generation)
            => Associate(type, new EventType(artifactId, generation));

        bool TryGetDecoratedEventType(Type type, out EventType eventType)
        {
            eventType = (type.GetCustomAttributes(typeof(EventTypeAttribute), true).FirstOrDefault() as EventTypeAttribute)?.EventType;
            return eventType != default;
        }
    }
}
