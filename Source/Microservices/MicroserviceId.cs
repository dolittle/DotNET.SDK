// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Microservices
{
    /// <summary>
    /// Represents the concept of a microservice.
    /// </summary>
    public class MicroserviceId : ConceptAs<Guid>
    {
        /// <summary>
        /// Represents the identifier for a not set microservice.
        /// </summary>
        public static readonly MicroserviceId NotSet = Guid.Parse("4a5d2bc3-543f-459a-ab0b-e8e924093260");

        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to a <see cref="MicroserviceId"/>.
        /// </summary>
        /// <param name="microserviceId"><see cref="Guid"/> representing the microservice.</param>
        public static implicit operator MicroserviceId(Guid microserviceId)
            => new MicroserviceId { Value = microserviceId };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to a <see cref="MicroserviceId"/>.
        /// </summary>
        /// <param name="microserviceId"><see cref="string"/> representing the microservice id.</param>
        public static implicit operator MicroserviceId(string microserviceId)
            => new MicroserviceId { Value = Guid.Parse(microserviceId) };

        /// <summary>
        /// Create a new <see cref="MicroserviceId"/> identifier.
        /// </summary>
        /// <returns><see cref="MicroserviceId"/>.</returns>
        public static MicroserviceId New()
            => new MicroserviceId { Value = Guid.NewGuid() };
    }
}
