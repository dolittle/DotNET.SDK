// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Represents the builder for configuring event handlers.
    /// </summary>
    public class ProjectionsBuilder
    {
        readonly IList<ICanBuildAndRegisterAProjection> _builders = new List<ICanBuildAndRegisterAProjection>();

        /// <summary>
        /// Start building an projection.
        /// </summary>
        /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
        /// <returns>The <see cref="ProjectionBuilder" /> for continuation.</returns>
        public ProjectionBuilder CreateProjection(ProjectionId projectionId)
        {
            var builder = new ProjectionBuilder(projectionId);
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
                    typeof(ConventionProjectionBuilder<>).MakeGenericType(type),
                    type) as ICanBuildAndRegisterAProjection;
            _builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Registers all projection classes from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to register the projection classes from.</param>
        /// <returns>The <see cref="ProjectionsBuilder" /> for continuation.</returns>
        public ProjectionsBuilder RegisterAllFrom(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes.Where(IsProjection))
            {
                RegisterProjection(type);
            }

            return this;
        }

        /// <summary>
        /// Build and registers projections.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            foreach (var builder in _builders)
            {
                builder.BuildAndRegister(eventProcessors, eventTypes, processingConverter, loggerFactory, cancellation);
            }
        }

        bool IsProjection(Type type)
            => type.GetCustomAttributes(typeof(ProjectionAttribute), true).FirstOrDefault() as ProjectionAttribute != default;
    }
}
