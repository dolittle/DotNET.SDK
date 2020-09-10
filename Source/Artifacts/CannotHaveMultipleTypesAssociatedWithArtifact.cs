// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="Artifact{TId}" /> is associated with multiple <see cref="Type" />.
    /// </summary>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    public class CannotHaveMultipleTypesAssociatedWithArtifact<TId> : Exception
        where TId : ArtifactId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotHaveMultipleTypesAssociatedWithArtifact{TId}"/> class.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact{TId}" /> that is associated with multiple <see cref="Type" />.</param>
        /// <param name="type">The <see cref="Type" /> the <see cref="Artifact{TId}" /> is attempted being associated to.</param>
        /// <param name="associatedType">The already associated <see cref="Type" />.</param>
        public CannotHaveMultipleTypesAssociatedWithArtifact(Artifact<TId> artifact, Type type, Type associatedType)
            : base($"{artifact} cannot be associated with {type} because it is already associated with {associatedType}")
        {
        }
    }
}
