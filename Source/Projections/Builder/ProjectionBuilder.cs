// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Represents a builder for building projections.
    /// </summary>
    public class ProjectionBuilder : ICanBuildAndRegisterAProjection
    {
        readonly ProjectionId _projectionId;
        ICanBuildAndRegisterAProjection _methodsBuilder;

        ScopeId _scopeId = ScopeId.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionBuilder"/> class.
        /// </summary>
        /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
        public ProjectionBuilder(ProjectionId projectionId)
        {
            _projectionId = projectionId;
        }

        /// <summary>
        /// Defines the projection to operate in a specific <see cref="_scopeId" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="_scopeId" />.</param>
        /// <returns>The builder for continuation.</returns>
        public ProjectionBuilder InScope(ScopeId scopeId)
        {
            _scopeId = scopeId;
            return this;
        }

        /// <summary>
        /// Creates a <see cref="ProjectionMethodsForReadModelBuilder{TReadModel}" /> for the specified read model type.
        /// </summary>
        /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
        /// <returns>The <see cref="ProjectionMethodsForReadModelBuilder{TReadModel}" /> for continuation.</returns>
        public ProjectionMethodsForReadModelBuilder<TReadModel> ForReadModel<TReadModel>()
            where TReadModel : class, new()
        {
            if (_methodsBuilder != default)
            {
                // Throw exceptionee c
            }

            var builder = new ProjectionMethodsForReadModelBuilder<TReadModel>(_projectionId, _scopeId);
            _methodsBuilder = builder;
            return builder;
        }

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            if (_methodsBuilder == null)
            {
                throw new ProjectionNeedsToBeForReadModel(_projectionId);
            }

            _methodsBuilder.BuildAndRegister(eventProcessors, eventTypes, processingConverter, loggerFactory, cancellation);
        }
    }
}
