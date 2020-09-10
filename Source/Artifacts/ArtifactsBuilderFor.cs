// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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
        /// <param name="artifacts">The <see cref="IArtifacts{TArtifact}" />.</param>
        protected ArtifactsBuilderFor(TArtifacts artifacts) => Artifacts = artifacts;

        /// <summary>
        /// Gets the <see cref="IArtifacts{TArtifact}" />.
        /// </summary>
        protected TArtifacts Artifacts { get; }

        /// <summary>
        /// Build an instance of <typeparamref name="TArtifacts" /> for <typeparamref name="TArtifact"/> artifact.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TArtifacts" />.</returns>
        public TArtifacts Build() => Artifacts;

        /// <summary>
        /// Associate the <see cref="Type" /> with an <typeparamref name="TArtifact"/> />.
        /// </summary>
        /// <param name="artifact">The <typeparamref name="TArtifact"/> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <typeparamref name="TArtifact"/>.</typeparam>
        /// <returns>The <typeparamref name="TBuilder" /> for building <typeparamref name="TArtifacts" />.</returns>
        public TBuilder Associate<T>(TArtifact artifact)
            where T : class
            => Associate(typeof(T), artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <typeparamref name="TArtifact"/>.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <typeparamref name="TBuilder" /> for building <typeparamref name="TArtifacts" />.</returns>
        public virtual TBuilder Associate(Type type, TArtifactId artifactId)
            => Associate(type, artifactId, Generation.First);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <typeparamref name="TArtifact"/>.</typeparam>
        /// <returns>The <typeparamref name="TBuilder" /> for building <typeparamref name="TArtifacts" />.</returns>
        public TBuilder Associate<T>(TArtifactId artifactId)
            where T : class
            => Associate(typeof(T), artifactId);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <typeparamref name="TArtifact"/>.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <typeparamref name="TArtifact"/>.</typeparam>
        /// <returns>The <typeparamref name="TBuilder" /> for building <typeparamref name="TArtifacts" />.</returns>
        public TBuilder Associate<T>(TArtifactId artifactId, Generation generation)
            where T : class
            => Associate(typeof(T), artifactId, generation);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <typeparamref name="TArtifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <typeparamref name="TArtifact" />.</param>
        /// <param name="artifact">The <typeparamref name="TArtifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <typeparamref name="TBuilder" /> for building <typeparamref name="TArtifacts" />.</returns>
        public abstract TBuilder Associate(Type type, TArtifact artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <typeparamref name="TArtifact"/>.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <typeparamref name="TArtifact"/>.</param>
        /// <returns>The <typeparamref name="TBuilder" /> for building <typeparamref name="TArtifacts" />.</returns>
        public abstract TBuilder Associate(Type type, TArtifactId artifactId, Generation generation);
    }
}
