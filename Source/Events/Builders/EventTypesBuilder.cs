// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Artifacts;
using NJsonSchema.Generation;

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
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate<T>(EventTypeId eventTypeId)
            where T : class
            => Associate(typeof(T), eventTypeId);

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate(Type type, EventTypeId eventTypeId)
            => Associate(type, new EventType(eventTypeId));

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="EventType" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate<T>(EventTypeId eventTypeId, Generation generation)
            where T : class
            => Associate(typeof(T), eventTypeId, generation);

        /// <summary>
        /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="EventType" />.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        public EventTypesBuilder Associate(Type type, EventTypeId eventTypeId, Generation generation)
            => Associate(type, new EventType(eventTypeId, generation));

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
        /// Associate a <see cref="Type" /> with the <see cref="EventType" /> given by an attribute.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to associate with an <see cref="EventType" />.</typeparam>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        /// <remarks>
        /// The type must have a eventType attribute.
        /// </remarks>
        public EventTypesBuilder RegisterForSchema<T>()
            where T : class
            => RegisterForSchema(typeof(T));

        /// <summary>
        /// Associate the <see cref="Type" /> with the <see cref="EventType" /> given by an attribute.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
        /// <returns>The <see cref="EventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
        /// <remarks>
        /// The type must have a eventType attribute.
        /// </remarks>
        public EventTypesBuilder RegisterForSchema(Type type)
        {
            ThrowIfTypeIsMissingEventTypeAttribute(type);
            TryGetEventTypeFromAttribute(type, out var eventType);
            AddAssociation(type, eventType);
            CreateSchema(type, eventType);
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

        void CreateSchema(Type type, EventType eventType)
        {
            var settings = new JsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(settings);
            var schema = generator.Generate(type);
            schema.ExtensionData = new Dictionary<string, object>
            {
                { "EventTypeId", eventType.Id.Value.ToString() },
                { "EventTypeGeneration", eventType.Generation.Value.ToString() },
                { "id", $"https://dolittle.io/{type}/{eventType.Generation.Value}" }
            };
            var schemaData = schema.ToJson();

            Directory.CreateDirectory("GeneratedJSON");

            File.WriteAllText($"GeneratedJSON/{type}.json", schemaData);
        }

        bool IsEventType(Type type)
            => type.GetCustomAttributes(typeof(EventTypeAttribute), true).FirstOrDefault() as EventTypeAttribute != default;

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

        void ThrowIfAttributeSpecifiesADifferentEventType(Type type, EventType providedType)
        {
            if (TryGetEventTypeFromAttribute(type, out var attributeType) && attributeType != providedType)
            {
                throw new ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(providedType, attributeType, type);
            }
        }

        bool TryGetEventTypeFromAttribute(Type type, out EventType eventType)
        {
            if (Attribute.GetCustomAttribute(type, typeof(EventTypeAttribute)) is EventTypeAttribute attribute)
            {
                eventType = attribute.EventType;
                return true;
            }

            eventType = default;
            return false;
        }
    }
}
