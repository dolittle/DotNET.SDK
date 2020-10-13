// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Execution
{
    /// <summary>
    /// A unique identifier to allow us to trace actions and their consequencies throughout the system.
    /// </summary>
    public class CorrelationId : ConceptAs<Guid>
    {
        /// <summary>
        /// The <see cref="CorrelationId"/> used by the system.
        /// </summary>
        public static readonly CorrelationId System = "868ff40f-a133-4d0f-bfdd-18d726181e01";

        /// <summary>
        /// Implicitly converts a <see cref="Guid" /> to an instance of <see cref="CorrelationId" />.
        /// </summary>
        /// <param name="correlationId">The value to initialize the <see cref="CorrelationId" /> with.</param>
        public static implicit operator CorrelationId(Guid correlationId) => new CorrelationId { Value = correlationId };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to a <see cref="CorrelationId"/>.
        /// </summary>
        /// <param name="correlationId"><see cref="string"/> representing the correlation id.</param>
        public static implicit operator CorrelationId(string correlationId) => new CorrelationId { Value = Guid.Parse(correlationId) };

        /// <summary>
        /// Creates a new <see cref="CorrelationId" /> with a generated Guid value.
        /// </summary>
        /// <returns>A <see cref="CorrelationId" /> initialised with a random Guid value.</returns>
        public static CorrelationId New() => Guid.NewGuid();
    }
}