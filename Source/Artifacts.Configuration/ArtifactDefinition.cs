// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents the definition of an artifact for configuration.
    /// </summary>
    public class ArtifactDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactDefinition"/> class.
        /// </summary>
        /// <param name="generation">Generation of the artifact.</param>
        /// <param name="type">The CLR type of the arfifact.</param>
        public ArtifactDefinition(ArtifactGeneration generation, ClrType type)
        {
            Generation = generation;
            Type = type;
        }

        /// <summary>
        /// Gets the <see cref="ArtifactGeneration">generation number</see> for the artifact.
        /// </summary>
        public ArtifactGeneration Generation { get; }

        /// <summary>
        /// Gets the <see cref="ClrType"/> represented by the artifact.
        /// </summary>
        public ClrType Type { get; }
    }
}