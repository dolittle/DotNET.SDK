// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="Artifact" /> has a missing association.
    /// </summary>
    public class UnknownType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownType"/> class.
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact" /> that has a missing association.</param>
        public UnknownType(Artifact artifact)
            : base($"{artifact} does not have a type associated")
        {
        }
    }
}
