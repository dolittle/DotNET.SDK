// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="Artifact" /> is associated with multiple <see cref="Type" />.
    /// </summary>
    public class CannotHaveMultipleTypesAssociatedWithArtifact : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotHaveMultipleTypesAssociatedWithArtifact"/> class.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact" /> that is associated with multiple <see cref="Type" />.</param>
        public CannotHaveMultipleTypesAssociatedWithArtifact(Artifact artifact)
            : base($"{artifact} cannot be associated with multiple types")
        {
        }
    }
}
