// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Represents the builder for configuring embeddings.
    /// </summary>
    public class EmbeddingsBuilder
    {
        readonly IList<ICanBuildAndRegisterAnEmbedding> _builders = new List<ICanBuildAndRegisterAnEmbedding>();
        readonly IProjectionReadModelTypeAssociations _projectionAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingsBuilder"/> class.
        /// </summary>
        /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypeAssociations"/>.</param>
        public EmbeddingsBuilder(IProjectionReadModelTypeAssociations projectionAssociations)
        {
            _projectionAssociations = projectionAssociations;
        }

        /// <summary>
        /// Start building an embedding.
        /// </summary>
        /// <param name="projectionId">The <see cref="EmbeddingId" />.</param>
        /// <returns>The <see cref="EmbeddingBuilder" /> for continuation.</returns>
        public EmbeddingBuilder CreateEmbedding(EmbeddingId projectionId)
        {
            var builder = new EmbeddingBuilder(projectionId, _projectionAssociations);
            _builders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Registers a <see cref="Type" /> as an embedding class.
        /// </summary>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the embedding class.</typeparam>
        /// <returns>The <see cref="EmbeddingsBuilder" /> for continuation.</returns>
        public EmbeddingsBuilder RegisterEmbedding<TProjection>()
            where TProjection : class, new()
            => RegisterEmbedding(typeof(TProjection));

        /// <summary>
        /// Registers a <see cref="Type" /> as an embedding class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the embedding.</param>
        /// <returns>The <see cref="EmbeddingsBuilder" /> for continuation.</returns>
        public EmbeddingsBuilder RegisterEmbedding(Type type)
        {
            var builder = Activator.CreateInstance(
                    typeof(ConventionEmbeddingBuilder<>).MakeGenericType(type))
                    as ICanBuildAndRegisterAnEmbedding;
            _builders.Add(builder);
            _projectionAssociations.Associate(type);
            return this;
        }

        /// <summary>
        /// Registers all embedding classes from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to register the embedding classes from.</param>
        /// <returns>The <see cref="EmbeddingsBuilder" /> for continuation.</returns>
        public EmbeddingsBuilder RegisterAllFrom(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes.Where(IsEmbedding))
            {
                RegisterEmbedding(type);
            }

            return this;
        }

        /// <summary>
        /// Build and registers projections.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventsToProtobufConverter">The <see cref="IConvertEventsToProtobuf" />.</param>
        /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IConvertEventsToProtobuf eventsToProtobufConverter,
            IConvertProjectionsToSDK projectionConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            foreach (var builder in _builders)
            {
                builder.BuildAndRegister(eventProcessors, eventTypes, eventsToProtobufConverter, projectionConverter, loggerFactory, cancellation);
            }
        }

        bool IsEmbedding(Type type)
            => (type.GetCustomAttributes(typeof(EmbeddingAttribute), true).FirstOrDefault() as EmbeddingAttribute) != default;
    }
}
