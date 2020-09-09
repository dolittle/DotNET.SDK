// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Defines a system that can build artifacts.
    /// </summary>
    /// <typeparam name="TArtifacts">The <see cref="Type" /> of the <see cref="IArtifacts{TArtifact}" />.</typeparam>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact" />.</typeparam>
    /// <typeparam name="TArtifactIdentifier">The <see cref="Type" /> of the unique identifier for the <see cref="Artifact" />.</typeparam>
    /// <typeparam name="TBuilder">The <see cref="Type" /> of the <see cref="IArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactIdentifier, TBuilder}" /> builder.</typeparam>
    public interface IArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactIdentifier, TBuilder>
        where TArtifacts : IArtifacts<TArtifact>
        where TArtifact : Artifact
        where TArtifactIdentifier : ArtifactId
        where TBuilder : IArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactIdentifier, TBuilder>
    {
        /// <summary>
        /// Build an instance of <see cref="IArtifacts{TArtifact}" /> for <typeparamref name="TArtifact"/> artifact.
        /// </summary>
        /// <returns>An instance of <see cref="IArtifacts{TArtifact}" />.</returns>
        TArtifacts Build();

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifact">The <see cref="Artifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate(Type type, TArtifact artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate<T>(TArtifact artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate(Type type, TArtifactIdentifier artifactId);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate<T>(TArtifactIdentifier artifactId);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate<T>(TArtifactIdentifier artifactId, Generation generation);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact" />.</param>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate(Type type, TArtifactIdentifier artifactId, Generation generation);
    }
}
