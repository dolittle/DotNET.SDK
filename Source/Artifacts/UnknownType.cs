// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="Artifact{TId}" /> has a missing association.
    /// </summary>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    public class UnknownType<TId> : Exception
        where TId : ArtifactId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownType{TId}"/> class.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact{TId}" /> that has a missing association.</param>
        public UnknownType(Artifact<TId> artifact)
            : base($"{artifact} does not have a type associated")
        {
        }
    }
}
