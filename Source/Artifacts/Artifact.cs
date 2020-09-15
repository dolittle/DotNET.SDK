// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents the base representation of an artifact.
    /// </summary>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    public abstract class Artifact<TId> : Value<Artifact<TId>>, IArtifact
        where TId : ArtifactId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Artifact{TId}"/> class.
        /// </summary>
        /// <param name="id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
        protected Artifact(TId id)
            : this(id, Generation.First)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifact{TId}"/> class.
        /// </summary>
        /// <param name="id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
        /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.</param>
        protected Artifact(TId id, Generation generation)
        {
            Id = id;
            Generation = generation;
        }

        /// <summary>
        /// Gets the <typeparamref name="TId" >unique identifier</typeparamref> of the <see cref="Artifact{TId}"/>.
        /// </summary>
        public TId Id { get; }

        /// <inheritdoc/>
        public Generation Generation { get; }

        /// <inheritdoc/>
        ArtifactId IArtifact.Id => Id;

        /// <inheritdoc/>
        public override string ToString() => $"{GetType().Name}(\"{Id}\", {Generation})";
    }
}
