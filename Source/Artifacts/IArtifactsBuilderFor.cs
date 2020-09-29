// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Defines a system that can build artifacts.
    /// </summary>
    /// <typeparam name="TArtifacts">The <see cref="Type" /> of the <see cref="IArtifacts{TArtifact}" />.</typeparam>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TArtifactId">The <see cref="Type" /> of the unique identifier for the <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TBuilder">The <see cref="Type" /> of the <see cref="IArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> builder.</typeparam>
    public interface IArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactId, TBuilder>
        where TArtifacts : IArtifacts<TArtifact>
        where TArtifact : Artifact<TArtifactId>
        where TArtifactId : ArtifactId
        where TBuilder : IArtifactsBuilderFor<TArtifacts, TArtifact, TArtifactId, TBuilder>
    {
        /// <summary>
        /// Build an instance of <see cref="IArtifacts{TArtifact}" /> for <typeparamref name="TArtifact"/> artifact.
        /// </summary>
        /// <param name="artifacts">The <typeparamref name="TArtifacts"/> to build upon.</param>
        void AddAssociationsInto(TArtifacts artifacts);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact{TId}" />.</param>
        /// <param name="artifact">The <see cref="Artifact{TId}" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate(Type type, TArtifact artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact{TId}" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact{TId}" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate<T>(TArtifact artifact)
            where T : class;

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact{TId}" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate(Type type, TArtifactId artifactId);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact{TId}" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate<T>(TArtifactId artifactId)
            where T : class;

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact{TId}" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact{TId}" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate<T>(TArtifactId artifactId, Generation generation)
            where T : class;

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact{TId}" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact{TId}" />.</param>
        /// <returns>The <see cref="ArtifactsBuilderFor{TArtifacts, TArtifact, TArtifactId, TBuilder}" /> for building <see cref="IArtifacts{TArtifact}" />.</returns>
        TBuilder Associate(Type type, TArtifactId artifactId, Generation generation);
    }
}
