// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateRootFactory" />.
    /// </summary>
    public class AggregateRootFactory : IAggregateRootFactory
    {
        readonly IEventTypes _eventTypes;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootFactory"/> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public AggregateRootFactory(IEventTypes eventTypes, ILoggerFactory loggerFactory)
        {
            _eventTypes = eventTypes;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public TAggregateRoot Create<TAggregateRoot>(EventSourceId eventSourceId)
            where TAggregateRoot : AggregateRoot
        {
            var aggregateRootType = typeof(TAggregateRoot);
            ThrowIfInvalidConstructor(aggregateRootType);
            var constructor = typeof(TAggregateRoot).GetConstructors().Single();

            var aggregateRoot = GetInstanceFrom<TAggregateRoot>(eventSourceId, constructor);
            ThrowIfCouldNotCreateAggregateRoot(aggregateRoot);
            return aggregateRoot;
        }

        TAggregateRoot GetInstanceFrom<TAggregateRoot>(EventSourceId id, ConstructorInfo constructor)
            where TAggregateRoot : AggregateRoot
            => constructor.Invoke(
                new object[]
                {
                    id,
                    _eventTypes
                }) as TAggregateRoot;

        void ThrowIfInvalidConstructor(Type type)
        {
            ThrowIfNotOneConstructor(type);
            ThrowIfConstructorIsInvalid(type, type.GetConstructors().Single());
        }

        void ThrowIfNotOneConstructor(Type type)
        {
            if (type.GetConstructors().Length != 1) ThrowInvalidConstructorSignature(type, "expected only a single constructor");
        }

        void ThrowIfConstructorIsInvalid(Type type, ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            ThrowIfIncorrectNumberOfParameter(type, parameters);
            ThrowIfIncorrectFirstParameter(type, parameters[0]);
            ThrowIfIncorrectSecondParameter(type, parameters[1]);
        }

        void ThrowIfIncorrectFirstParameter(Type type, ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            if (parameterType != typeof(Guid) || parameterType != typeof(EventSourceId))
                throw new InvalidAggregateRootConstructorSignature(type, $"expected type of first parameter to be {typeof(Guid)} or {typeof(EventSourceId)}");
        }

        void ThrowIfIncorrectSecondParameter(Type type, ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            if (parameterType != typeof(IEventTypes))
                throw new InvalidAggregateRootConstructorSignature(type, $"expected type of first parameter to be {typeof(IEventTypes)}");
        }

        void ThrowIfIncorrectNumberOfParameter(Type type, ParameterInfo[] parameters)
        {
            const int expectedNumberParameters = 2;
            if (parameters.Length != expectedNumberParameters)
                throw new InvalidAggregateRootConstructorSignature(type, $"expected {expectedNumberParameters} parameters");
        }

        void ThrowIfCouldNotCreateAggregateRoot<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot
        {
            if (aggregateRoot == default) throw new CouldNotCreateAggregateRootInstance(typeof(TAggregateRoot));
        }

        void ThrowInvalidConstructorSignature(Type type, string expectations)
            => throw new InvalidAggregateRootConstructorSignature(type, expectations);
    }
}
