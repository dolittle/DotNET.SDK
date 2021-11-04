// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders
{
    /// <summary>
    /// Represents a system that registers <see cref="Type" /> to <see cref="EventType" /> associations to an <see cref="IEventTypes" /> instance.
    /// </summary>
    public class AggregateRootsBuilder
    {
        readonly List<(Type, AggregateRootType)> _associations = new List<(Type, AggregateRootType)>();

        /// <summary>
        /// Associate a <see cref="Type" /> with the <see cref="AggregateRootType" /> given by an attribute.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to associate with an <see cref="AggregateRootType" />.</typeparam>
        /// <returns>The <see cref="AggregateRootsBuilder"/> for continuation.</returns>
        /// <remarks>
        /// The type must have a AggregateRoot attribute.
        /// </remarks>
        public AggregateRootsBuilder Register<T>()
            where T : class
            => Register(typeof(T));

        /// <summary>
        /// Associate the <see cref="Type" /> with the <see cref="AggregateRootType" /> given by an attribute.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="AggregateRootType" />.</param>
        /// <returns>The <see cref="AggregateRootsBuilder"/> for continuation.</returns>
        /// <remarks>
        /// The type must have a AggregateRoot attribute.
        /// </remarks>
        public AggregateRootsBuilder Register(Type type)
        {
            ThrowIfTypeIsMissingAggregateRootAttribute(type);
            TryGetAggregateRootTypeFromAttribute(type, out var eventType);
            AddAssociation(type, eventType);
            return this;
        }

        /// <summary>
        /// Registers all aggregate root classes from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to register the aggregate root classes from.</param>
        /// <returns>The <see cref="AggregateRootsBuilder" /> for continuation.</returns>
        public AggregateRootsBuilder RegisterAllFrom(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes.Where(IsAggregateRoot))
            {
                Register(type);
            }

            return this;
        }

        /// <summary>
        /// Builds the aggregate roots by registering them with the Runtime.
        /// </summary>
        /// <param name="aggregateRoots">The <see cref="Internal.AggregateRootsClient"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task BuildAndRegister(Internal.AggregateRootsClient aggregateRoots, CancellationToken cancellationToken)
            => aggregateRoots.Register(_associations.Select(_ => _.Item2), cancellationToken);

        static bool IsAggregateRoot(Type type)
            => type.GetCustomAttributes(typeof(AggregateRootAttribute), true).FirstOrDefault() is AggregateRootAttribute;

        static bool TryGetAggregateRootTypeFromAttribute(Type type, out AggregateRootType aggregateRootType)
        {
            if (Attribute.GetCustomAttribute(type, typeof(AggregateRootAttribute)) is AggregateRootAttribute attribute)
            {
                if (!attribute.Type.HasAlias)
                {
                    aggregateRootType = new AggregateRootType(attribute.Type.Id, attribute.Type.Generation, type.Name);
                    return true;
                }

                aggregateRootType = attribute.Type;
                return true;
            }

            aggregateRootType = default;
            return false;
        }

        void AddAssociation(Type type, AggregateRootType aggregateRootType)
        {
            _associations.Add((type, aggregateRootType));
        }

        void ThrowIfTypeIsMissingAggregateRootAttribute(Type type)
        {
            if (!TryGetAggregateRootTypeFromAttribute(type, out _))
            {
                throw new TypeIsMissingAggregateRootAttribute(type);
            }
        }
    }
}
