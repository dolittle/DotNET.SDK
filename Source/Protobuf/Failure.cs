// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;
using Contracts = Dolittle.Protobuf.Contracts;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Represents a failure.
    /// </summary>
    public class Failure : Value<Failure>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Failure"/> class.
        /// </summary>
        /// <param name="id"><see cref="FailureId" />.</param>
        /// <param name="reason"><see cref="FailureReason" />.</param>
        public Failure(FailureId id, FailureReason reason)
        {
            Id = id;
            Reason = reason;
        }

        /// <summary>
        /// Gets the <see cref="FailureId" />.
        /// </summary>
        public FailureId Id { get; }

        /// <summary>
        /// Gets the <see cref="FailureReason" />.
        /// </summary>
        public FailureReason Reason { get; }

        /// <summary>
        /// Implicitly convert <see cref="Failure" /> to <see cref="Contracts.Failure" />.
        /// </summary>
        /// <param name="failure"><see cref="Failure" /> to convert.</param>
        public static implicit operator Contracts.Failure(Failure failure)
        {
            if (failure == null)
                return null;

            return new Contracts.Failure
            {
                Id = failure.Id == null ? FailureId.Undocumented.ToProtobuf() : failure.Id.ToProtobuf(),
                Reason = failure.Reason,
            };
        }

        /// <summary>
        /// Implicitly convert <see cref="Contracts.Failure" /> to <see cref="Failure" />.
        /// </summary>
        /// <param name="failure"><see cref="Contracts.Failure" /> to convert.</param>
        public static implicit operator Failure(Contracts.Failure failure)
        {
            if (failure == null)
                return null;

            if (!failure.Id.TryTo<FailureId>(out var id, out var _))
                return new Failure(FailureId.Undocumented, failure.Reason);

            return new Failure(id, failure.Reason);
        }
    }
}
