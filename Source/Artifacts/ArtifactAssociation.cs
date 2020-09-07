// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents the association between a <see cref="Type" /> and an <see cref="Artifact" />.
    /// </summary>
    public class ArtifactAssociation : Value<ArtifactAssociation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactAssociation"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" />.</param>
        /// <param name="artifact">The <see cref="Artifact" />.</param>
        public ArtifactAssociation(Type type, Artifact artifact)
        {
            Type = type;
            Artifact = artifact;
        }

        /// <summary>
        /// Gets the <see cref="Type" />.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="Artifact" />.
        /// </summary>
        public Artifact Artifact { get; }
    }
}
