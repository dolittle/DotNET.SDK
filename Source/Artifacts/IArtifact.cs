// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Defines an artifact.
    /// </summary>
    public interface IArtifact
    {
        /// <summary>
        /// Gets the <see cref="ArtifactId" >unique identifier</see> of the <see cref="IArtifact"/>.
        /// </summary>
        ArtifactId Id { get; }

        /// <summary>
        /// Gets the <see cref="Generation">generation</see> of the <see cref="Artifact{TId}"/>.
        /// </summary>
        Generation Generation { get; }
    }
}
