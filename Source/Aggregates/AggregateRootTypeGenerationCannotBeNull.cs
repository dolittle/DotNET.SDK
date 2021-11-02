// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="AggregateRootType"/> with a <see cref="Generation"/> that is null.
    /// </summary>
    public class AggregateRootTypeGenerationCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootTypeGenerationCannotBeNull"/> class.
        /// </summary>
        public AggregateRootTypeGenerationCannotBeNull()
            : base($"The {nameof(Generation)} of an {nameof(AggregateRootType)} cannot be null")
        {
        }
    }
}