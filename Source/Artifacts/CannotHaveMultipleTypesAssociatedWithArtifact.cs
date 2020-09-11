// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IArtifact" /> is associated with multiple <see cref="Type" />.
    /// </summary>
    public class CannotHaveMultipleTypesAssociatedWithArtifact : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotHaveMultipleTypesAssociatedWithArtifact"/> class.
        /// </summary>
        /// <param name="artifact">The <see cref="IArtifact" /> that is associated with multiple <see cref="Type" />.</param>
        /// <param name="type">The <see cref="Type" /> the <see cref="Artifact{TId}" /> is attempted being associated to.</param>
        /// <param name="associatedType">The already associated <see cref="Type" />.</param>
        public CannotHaveMultipleTypesAssociatedWithArtifact(IArtifact artifact, Type type, Type associatedType)
            : base($"{artifact} cannot be associated with {type} because it is already associated with {associatedType}")
        {
        }
    }
}
