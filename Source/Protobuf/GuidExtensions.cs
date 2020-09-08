// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Concepts;
using Google.Protobuf;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Conversion extensions for converting between <see cref="Guid"/> and <see cref="Uuid"/>.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Convert a <see cref="Guid"/> to a <see cref="Uuid"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> to convert.</param>
        /// <returns>The converted <see cref="Uuid"/>.</returns>
        public static Uuid ToProtobuf(this Guid id) => new Uuid { Value = ByteString.CopyFrom(id.ToByteArray()) };

        /// <summary>
        /// Convert a <see cref="ConceptAs{Guid}"/> to a <see cref="Uuid"/>.
        /// </summary>
        /// <param name="id">The <see cref="ConceptAs{Guid}"/> to convert.</param>
        /// <returns>The converted <see cref="Uuid"/>.</returns>
        public static Uuid ToProtobuf(this ConceptAs<Guid> id) => id.Value.ToProtobuf();

        /// <summary>
        /// Convert a <see cref="Uuid"/> to a <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The <see cref="Uuid"/> to convert.</param>
        /// <returns>The converted <see cref="Guid"/>.</returns>
        public static Guid ToGuid(this Uuid id) => new Guid(id.Value.ToByteArray());

        /// <summary>
        /// Convert a <see cref="Uuid"/> to a <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="id">The <see cref="Uuid"/> to convert.</param>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <returns>The converted <see cref="ConceptAs{T}"/>.</returns>
        public static Guid To<T>(this Uuid id)
            where T : ConceptAs<Guid>, new()
            => new T { Value = id.ToGuid() };
    }
}