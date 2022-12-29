// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Events
{
    /// <summary>
    /// Represents a mock of <see cref="IEventTypes"/> that attempts to resolve <see cref="EventType"/> from attributes
    /// when they are needed at runtime.
    /// </summary>
    public class EventTypesMock : IEventTypes
    {
        readonly Dictionary<Type, EventType> _types = new Dictionary<Type, EventType>();

        /// <inheritdoc />
        public IEnumerable<EventType> All => _types.Values;

        /// <inheritdoc />
        public bool HasFor<T>()
            where T : class
            => HasFor(typeof(T));

        /// <inheritdoc />
        public bool HasFor(Type type)
        {
            if (_types.ContainsKey(type))
            {
                return true;
            }

            AssociateIfTypeHasAttribute(type);
            return _types.ContainsKey(type);
        }

        /// <inheritdoc />
        public EventType GetFor<T>()
            where T : class
            => GetFor(typeof(T));

        /// <inheritdoc />
        public EventType GetFor(Type type)
        {
            if (_types.ContainsKey(type))
            {
                return _types[type];
            }

            AssociateIfTypeHasAttribute(type);
            return _types[type];
        }

        /// <inheritdoc />
        public bool HasTypeFor(EventType artifact)
            => _types.ContainsValue(artifact);

        /// <inheritdoc />
        public Type GetTypeFor(EventType artifact)
            => _types.First(_ => _.Value == artifact).Key;

        /// <inheritdoc />
        public void Associate(Type type, EventType artifact)
            => _types.Add(type, artifact);

        void AssociateIfTypeHasAttribute(Type type)
        {
            if (!(Attribute.GetCustomAttribute(type, typeof(EventTypeAttribute)) is EventTypeAttribute attribute))
            {
                return;
            }

            var eventType = new EventType(attribute.Identifier, attribute.Generation);
            Associate(type, eventType);
        }
    }
}