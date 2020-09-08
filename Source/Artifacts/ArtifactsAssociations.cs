// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents a system that associates <see cref="IArtifacts" /> to an <see cref="IArtifacts" /> instances.
    /// </summary>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of <see cref="Artifact" /> to build.</typeparam>
    /// <typeparam name="TArtifactIdentifier">The <see cref="Type" /> of <see cref="ArtifactId" /> that identifies the <typeparamref name="TArtifact"/>.</typeparam>
    public abstract class ArtifactsAssociations<TArtifact, TArtifactIdentifier>
        where TArtifact : Artifact
        where TArtifactIdentifier : ArtifactId
    {
        readonly IArtifacts _artifacts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactsAssociations{TArtifact, TArtifactId}"/> class.
        /// </summary>
        /// <param name="artifacts">The <see cref="IArtifacts" />.</param>
        protected ArtifactsAssociations(IArtifacts artifacts)
        {
            _artifacts = artifacts;
        }

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifact">The <see cref="Artifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsAssociations{TArtifact, TArtifactId}" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsAssociations<TArtifact, TArtifactIdentifier> Associate(Type type, TArtifact artifact)
        {
            _artifacts.Associate(type, artifact);
            return this;
        }

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsAssociations{TArtifact, TArtifactId}" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsAssociations<TArtifact, TArtifactIdentifier> Associate<T>(TArtifact artifact)
            => Associate(typeof(T), artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsAssociations{TArtifact, TArtifactId}" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsAssociations<TArtifact, TArtifactIdentifier> Associate(Type type, TArtifactIdentifier artifactId)
            => Associate(type, artifactId, Generation.First);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsAssociations{TArtifact, TArtifactId}" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsAssociations<TArtifact, TArtifactIdentifier> Associate<T>(TArtifactIdentifier artifactId)
            => Associate<T>(artifactId, Generation.First);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsAssociations{TArtifact, TArtifactId}" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsAssociations<TArtifact, TArtifactIdentifier> Associate<T>(TArtifactIdentifier artifactId, Generation generation)
            => Associate(typeof(T), artifactId, generation);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact" />.</param>
        /// <returns>The <see cref="ArtifactsAssociations{TArtifact, TArtifactId}" /> for building <see cref="IArtifacts" />.</returns>
        public abstract ArtifactsAssociations<TArtifact, TArtifactIdentifier> Associate(Type type, TArtifactIdentifier artifactId, Generation generation);
    }
}
