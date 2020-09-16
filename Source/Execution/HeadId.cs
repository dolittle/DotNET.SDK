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
        /// A static singleton instance to represent a "NotSet" <see cref="HeadId"/>.
        /// </summary>
        public static readonly HeadId NotSet = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadId"/> class.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> value.</param>
        public HeadId(Guid id) => Value = id;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to an <see cref="HeadId"/>.
        /// </summary>
        /// <param name="id">HeadId as <see cref="Guid"/>.</param>
        public static implicit operator HeadId(Guid id) => new HeadId(id);

        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="HeadId"/>.
        /// </summary>
        /// <param name="idString">HeadId as <see cref="string"/>.</param>
        public static implicit operator HeadId(string idString) => new HeadId(Guid.Parse(idString));
    }
}
