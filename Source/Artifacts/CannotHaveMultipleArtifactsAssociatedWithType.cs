// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Type" /> is associated with multiple <see cref="IArtifact" />.
    /// </summary>
    public class CannotHaveMultipleArtifactsAssociatedWithType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotHaveMultipleArtifactsAssociatedWithType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that is associated with multiple <see cref="Artifact{TId}" />.</param>
        /// <param name="artifact">The <see cref="Artifact{TId}" /> the <see cref="Type" /> is attempted being associated to.</param>
        /// <param name="associatedArtifact">The already associated <see cref="Artifact{TId}" />.</param>
        public CannotHaveMultipleArtifactsAssociatedWithType(Type type, IArtifact artifact, IArtifact associatedArtifact)
            : base($"{type} cannot be associated with {artifact} because it is already associated with {associatedArtifact}")
        {
        }
    }
}
