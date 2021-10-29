// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateRootTypes" />.
    /// </summary>
    public class AggregateRootTypes : Artifacts<AggregateRootType, AggregateRootId>, IAggregateRootTypes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootTypes"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateRootTypes(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        protected override Exception CreateNoArtifactAssociatedWithType(Type type)
            => new AggregateRootTypeAssociatedWithType(type);

        /// <inheritdoc/>
        protected override Exception CreateNoTypeAssociatedWithArtifact(AggregateRootType artifact)
            => new NoTypeAssociatedWithAggregateRootType(artifact);

        /// <inheritdoc/>
        protected override Exception CreateCannotAssociateMultipleArtifactsWithType(Type type, AggregateRootType artifact, AggregateRootType existing)
            => new CannotAssociateMultipleAggregateRootTypesWithType(type, artifact, existing);

        /// <inheritdoc/>
        protected override Exception CreateCannotAssociateMultipleTypesWithArtifact(AggregateRootType artifact, Type type, Type existing)
            => new CannotAssociateMultipleTypesWithAggregateRootType(artifact, type, existing);
    }
}
