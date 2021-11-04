// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events.Builders
{
    /// <summary>
    /// Represents a system that registers <see cref="Type" /> to <see cref="EventType" /> associations to an <see cref="IEventTypes" /> instance.
    /// </summary>
    public class EventTypesBuilder
    {
        readonly List<(Type, EventType)> _associations = new List<(Type, EventType)>();

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate<T>(EventType eventType)
            where T : class
            => Associate(typeof(T), eventType);

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <param name="eventType">The <see cref="EventType" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate(Type type, EventType eventType)
        {
            AddAssociation(type, eventType);
            return this;
        }

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate<T>(EventTypeId eventTypeId, EventTypeAlias alias = default)
            where T : class
            => Associate(typeof(T), eventTypeId, alias);

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate(Type type, EventTypeId eventTypeId, EventTypeAlias alias = default)
            => Associate(type, new EventType(eventTypeId, alias));

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="EventType" />.</param>
        /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate<T>(EventTypeId eventTypeId, Generation generation, EventTypeAlias alias = default)
            where T : class
            => Associate(typeof(T), eventTypeId, generation, alias);

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="EventType" />.</param>
        /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate(Type type, EventTypeId eventTypeId, Generation generation, EventTypeAlias alias = default)
            => Associate(type, new EventType(eventTypeId, generation, alias));

        /// <summary>
        /// Associate a <see cref="Type" /> with the <see cref="EventType" /> given by an attribute.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to associate with an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        /// <remarks>
        /// The type must have a eventType attribute.
        /// </remarks>
        public EventTypesBuilder Register<T>()
            where T : class
            => Register(typeof(T));

        /// <summary>
        /// Associate the <see cref="Type" /> with the <see cref="EventType" /> given by an attribute.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        /// <remarks>
        /// The type must have a eventType attribute.
        /// </remarks>
        public EventTypesBuilder Register(Type type)
        {
            ThrowIfTypeIsMissingEventTypeAttribute(type);
            TryGetEventTypeFromAttribute(type, out var eventType);
            AddAssociation(type, eventType);
            return this;
        }

        /// <summary>
        /// Registers all event type classes from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to register the event type classes from.</param>
        /// <returns>The <see cref="EventTypesBuilder" /> for continuation.</returns>
        public EventTypesBuilder RegisterAllFrom(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes.Where(IsEventType))
            {
                Register(type);
            }

            return this;
        }

        /// <summary>
        /// Builds the event types by registering them with the Runtime.
        /// </summary>
        /// <param name="eventTypes">The <see cref="Internal.EventTypesClient"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task BuildAndRegister(Internal.EventTypesClient eventTypes, CancellationToken cancellationToken)
            => eventTypes.Register(_associations.Select(_ => _.Item2), cancellationToken);

        /// <summary>
        /// Adds all the <see cref="Type" /> to <see cref="EventType" /> associations to the provided <see cref="IEventTypes" />.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes"/> to add associations to.</param>
        public void AddAssociationsInto(IEventTypes eventTypes)
        {
            foreach (var (type, eventType) in _associations)
            {
                eventTypes.Associate(type, eventType);
            }
        }

        static bool IsEventType(Type type)
            => type.GetCustomAttributes(typeof(EventTypeAttribute), true).FirstOrDefault() is EventTypeAttribute;

        static void ThrowIfAttributeSpecifiesADifferentEventType(Type type, EventType providedType)
        {
            if (TryGetEventTypeFromAttribute(type, out var attributeType) && attributeType != providedType)
            {
                throw new ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(providedType, attributeType, type);
            }
        }

        static bool TryGetEventTypeFromAttribute(Type type, out EventType eventType)
        {
            if (Attribute.GetCustomAttribute(type, typeof(EventTypeAttribute)) is EventTypeAttribute attribute)
            {
                if (!attribute.EventType.HasAlias)
                {
                    eventType = new EventType(attribute.EventType.Id, attribute.EventType.Generation, type.Name);
                    return true;
                }

                eventType = attribute.EventType;
                return true;
            }

            eventType = default;
            return false;
        }

        void AddAssociation(Type type, EventType eventType)
        {
            ThrowIfAttributeSpecifiesADifferentEventType(type, eventType);
            _associations.Add((type, eventType));
        }

        void ThrowIfTypeIsMissingEventTypeAttribute(Type type)
        {
            if (!TryGetEventTypeFromAttribute(type, out _))
            {
                throw new TypeIsMissingEventTypeAttribute(type);
            }
        }
    }
}
