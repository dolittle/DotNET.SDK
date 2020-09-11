// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Type" /> does not have an artifact association.
    /// </summary>
    public class UnknownArtifact : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownArtifact"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that has a missing association.</param>
        public UnknownArtifact(Type type)
            : base($"{type} does not have an artifact association")
        {
        }
    }
}
