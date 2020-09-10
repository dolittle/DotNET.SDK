// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents the base representation of an artifact.
    /// </summary>
    public abstract class Artifact : Value<Artifact>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Artifact"/> class.
        /// </summary>
        /// <param name="id"><see cref="ArtifactId">Id</see> of the <see cref="Artifact"/>.</param>
        protected Artifact(ArtifactId id)
            : this(id, Generation.First)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifact"/> class.
        /// </summary>
        /// <param name="id"><see cref="ArtifactId">Id</see> of the <see cref="Artifact"/>.</param>
        /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="Artifact"/>.</param>
        protected Artifact(ArtifactId id, Generation generation)
        {
            Id = id;
            Generation = generation;
        }

        /// <summary>
        /// Gets the <see cref="Guid">unique identifier</see> of the <see cref="Artifact"/>.
        /// </summary>
        public ArtifactId Id { get; }

        /// <summary>
        /// Gets the <see cref="Generation">generation</see> of the <see cref="Artifact"/>.
        /// </summary>
        public Generation Generation { get; }

        /// <inheritdoc/>
        public override string ToString() => $"{GetType().Name}(\"{Id}\", {Generation})";
    }
}
