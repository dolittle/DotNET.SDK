// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Type" /> is associated with multiple <see cref="Artifact" />.
    /// </summary>
    public class CannotHaveMultipleArtifactsAssociatedWithType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotHaveMultipleArtifactsAssociatedWithType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that is associated with multiple <see cref="Artifact" />.</param>
        /// <param name="artifact">The <see cref="Artifact" /> the <see cref="Type" /> is attempted being associated to.</param>
        /// <param name="associatedArtifact">The already associated <see cref="Artifact" />.</param>
        public CannotHaveMultipleArtifactsAssociatedWithType(Type type, Artifact artifact, Artifact associatedArtifact)
            : base($"{type} cannot be associated with {artifact} because it is already associated with {associatedArtifact}. A type cannot be associated with multiple artifacts")
        {
        }
    }
}
