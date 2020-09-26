// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents a system that registers <see cref="Type" /> to <see cref="Artifact{TId}" /> associations to an <see cref="IArtifacts{TArtifact}" /> instance.
    /// </summary>
    /// <typeparam name="TArtifacts">The <see cref="Type" /> of <see cref="IArtifacts{TArtifact}" /> to build.</typeparam>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of <see cref="Artifact{TId}" /> to associate.</typeparam>
    /// <typeparam name="TArtifactId">The <see cref="Type" /> of <see cref="ArtifactId" /> that identifies the <typeparamref name="TArtifact"/>.</typeparam>
    /// <typeparam name="TBuilder">The <see cref="Type" /> of <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" />.</typeparam>
    public abstract class ArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactId, TBuilder> : IArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactId, TBuilder>
        where TArtifacts : IArtifacts<TArtifact>
        where TArtifact : Artifact<TArtifactId>
        where TArtifactId : ArtifactId
        where TBuilder : IArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactId, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}"/> class.
        /// </summary>
        protected ArtifactsBuilderFor() => Associations = new List<(Type, TArtifact)>();

        /// <summary>
        /// Gets all the associations to build.
        /// </summary>
        protected IList<(Type, TArtifact)> Associations { get; }

        /// <inheritdoc/>
        public TArtifacts Build(TArtifacts artifacts)
        {
            foreach (var (type, artifact) in Associations) artifacts.Associate(type, artifact);
            return artifacts;
        }

        /// <inheritdoc/>
        public TBuilder Associate<T>(TArtifact artifact)
            where T : class
            => Associate(typeof(T), artifact);

        /// <inheritdoc/>
        public virtual TBuilder Associate(Type type, TArtifactId artifactId)
            => Associate(type, artifactId, Generation.First);

        /// <inheritdoc/>
        public TBuilder Associate<T>(TArtifactId artifactId)
            where T : class
            => Associate(typeof(T), artifactId);

        /// <inheritdoc/>
        public TBuilder Associate<T>(TArtifactId artifactId, Generation generation)
            where T : class
            => Associate(typeof(T), artifactId, generation);

        /// <inheritdoc/>
        public abstract TBuilder Associate(Type type, TArtifact artifact);

        /// <inheritdoc/>
        public abstract TBuilder Associate(Type type, TArtifactId artifactId, Generation generation);
    }
}
