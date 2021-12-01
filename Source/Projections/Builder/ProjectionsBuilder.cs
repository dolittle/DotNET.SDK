// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Represents the builder for configuring event handlers.
    /// </summary>
    public class ProjectionsBuilder
    {
        readonly List<ICanBuildAndRegisterAProjection> _builders = new List<ICanBuildAndRegisterAProjection>();
        readonly Dictionary<Type, ICanBuildAndRegisterAProjection> _typedBuilders = new Dictionary<Type, ICanBuildAndRegisterAProjection>();
        readonly IProjectionReadModelTypeAssociations _projectionAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionsBuilder"/> class.
        /// </summary>
        /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypeAssociations"/>.</param>
        public ProjectionsBuilder(IProjectionReadModelTypeAssociations projectionAssociations)
        {
            _projectionAssociations = projectionAssociations;
        }

        /// <summary>
        /// Start building an projection.
        /// </summary>
        /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
        /// <returns>The <see cref="ProjectionBuilder" /> for continuation.</returns>
        public ProjectionBuilder CreateProjection(ProjectionId projectionId)
        {
            var builder = new ProjectionBuilder(projectionId, _projectionAssociations);
            _builders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Registers a <see cref="Type" /> as a projection class.
        /// </summary>
        /// <typeparam name="TProjection">The <see cref="Type" /> that is the projection class.</typeparam>
        /// <returns>The <see cref="ProjectionsBuilder" /> for continuation.</returns>
        public ProjectionsBuilder RegisterProjection<TProjection>()
            where TProjection : class, new()
            => RegisterProjection(typeof(TProjection));

        /// <summary>
        /// Registers a <see cref="Type" /> as a projection class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the projection.</param>
        /// <returns>The <see cref="ProjectionsBuilder" /> for continuation.</returns>
        public ProjectionsBuilder RegisterProjection(Type type)
        {
            var builder = Activator.CreateInstance(
                    typeof(ConventionProjectionBuilder<>).MakeGenericType(type))
                    as ICanBuildAndRegisterAProjection;
            _typedBuilders[type] = builder;
            _projectionAssociations.Associate(type);
            return this;
        }

        /// <summary>
        /// Registers all projection classes from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to register the projection classes from.</param>
        /// <returns>The <see cref="ProjectionsBuilder" /> for continuation.</returns>
        public ProjectionsBuilder RegisterAllFrom(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                if (IsProjection(type))
                {
                    RegisterProjection(type);
                }
            }

            return this;
        }

        /// <summary>
        /// Build and registers projections.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancelConnectToken">The <see cref="CancellationToken" />.</param>
        /// <param name="stopProcessingToken">The <see cref="CancellationToken" /> for stopping processing.</param>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IConvertProjectionsToSDK projectionConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancelConnectToken,
            CancellationToken stopProcessingToken)
        {
            foreach (var builder in _builders.Concat(_typedBuilders.Values))
            {
                builder.BuildAndRegister(eventProcessors, eventTypes, processingConverter, projectionConverter, loggerFactory, cancelConnectToken, stopProcessingToken);
            }
        }

        static bool IsProjection(Type type)
            => type.GetCustomAttributes(typeof(ProjectionAttribute), true).FirstOrDefault() is ProjectionAttribute;
    }
}
