// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Exception that gets thrown when trying to define another readmodel for a projection.
    /// </summary>
    public class ReadModelAlreadyDefinedForProjection : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadModelAlreadyDefinedForProjection"/> class.
        /// </summary>
        /// <param name="projectionId">The <see cref="EmbeddingId"/>.</param>
        /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
        /// <param name="type">The type of the readmodel already defined.</param>
        public ReadModelAlreadyDefinedForProjection(EmbeddingId projectionId, ScopeId scopeId, Type type)
            : base($"Projection {projectionId} in scope {scopeId} already has a readmodel of type {type} defined for it.")
        {
        }
    }
}
