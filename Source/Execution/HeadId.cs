// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Execution
{
    /// <summary>
    /// Represents the identification of an application.
    /// </summary>
    public class HeadId : ConceptAs<Guid>
    {
        /// <summary>
        /// The <see cref="HeadId"/> used when an identifier has not been defined.
        /// </summary>
        public static readonly HeadId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to an <see cref="HeadId"/>.
        /// </summary>
        /// <param name="headId">HeadId as <see cref="Guid"/>.</param>
        public static implicit operator HeadId(Guid headId) => new HeadId { Value = headId };

        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="HeadId"/>.
        /// </summary>
        /// <param name="headId">HeadId as <see cref="string"/>.</param>
        public static implicit operator HeadId(string headId) => new HeadId { Value = Guid.Parse(headId) };
    }
}
