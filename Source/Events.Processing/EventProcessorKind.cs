// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Represents the concept of a event processor kind.
    /// </summary>
    public record EventProcessorKind(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="EventProcessorKind"/>.
        /// </summary>
        /// <param name="kind">The event processor kind string.</param>
        public static implicit operator EventProcessorKind(string kind)
            => new(kind);
    }
}
