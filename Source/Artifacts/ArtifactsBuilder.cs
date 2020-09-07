// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents for building <see cref="IArtifacts" /> instances.
    /// </summary>
    public class ArtifactsBuilder
    {
        readonly IDictionary<Type, Artifact> _associations = new Dictionary<Type, Artifact>();

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifact">The <see cref="Artifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsBuilder" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsBuilder Associate(Type type, Artifact artifact)
        {
            if (!_associations.TryAdd(type, artifact)) throw new TypeCannotHaveMultipleArtifactAssociations(type);
            return this;
        }

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilder" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsBuilder Associate<T>(Artifact artifact) => Associate(typeof(T), artifact);

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <returns>The <see cref="ArtifactsBuilder" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsBuilder Associate(Type type, ArtifactId artifactId) => Associate(type, new Artifact(artifactId));

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilder" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsBuilder Associate<T>(ArtifactId artifactId) => Associate<T>(new Artifact(artifactId));

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to associate with an <see cref="Artifact" />.</param>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact" />.</param>
        /// <returns>The <see cref="ArtifactsBuilder" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsBuilder Associate(Type type, ArtifactId artifactId, Generation generation) => Associate(type, new Artifact(artifactId, generation));

        /// <summary>
        /// Associate the <see cref="Type" /> with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="artifactId">The <see cref="ArtifactId" /> that the <see cref="Type" /> is associated to.</param>
        /// <param name="generation">The <see cref="Generation" /> of the <see cref="Artifact" />.</param>
        /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="Artifact" />.</typeparam>
        /// <returns>The <see cref="ArtifactsBuilder" /> for building <see cref="IArtifacts" />.</returns>
        public ArtifactsBuilder Associate<T>(ArtifactId artifactId, Generation generation) => Associate<T>(new Artifact(artifactId, generation));

        /// <summary>
        /// Build an <see cref="IArtifacts" /> instance.
        /// </summary>
        /// <returns>The <see cref="IArtifacts" /> with associations.</returns>
        public IArtifacts Build() => new Artifacts(_associations);
    }
}
