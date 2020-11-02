// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Contracts = Dolittle.Protobuf.Contracts;

namespace Dolittle.SDK.Failures
{
    /// <summary>
    /// Extension methods for <see cref="Failure"/>.
    /// </summary>
    public static class FailureExtensions
    {
        /// <summary>
        /// Converts <see cref="Contracts.Failure"/> to a <see cref="Failure"/>.
        /// </summary>
        /// <param name="failure"><see cref="Contracts.Failure" /> to convert.</param>
        /// <returns>The converted <see cref="Failure"/>.</returns>
        public static Failure ToSDK(this Contracts.Failure failure)
        {
            if (failure == null)
                return null;

            if (!failure.Id.TryTo<FailureId>(out var id, out var _))
                return new Failure(FailureId.Undocumented, failure.Reason);

            return new Failure(id, failure.Reason);
        }

        /// <summary>
        /// Converts a <see cref="Failure"/> returned from the Runtime to an <see cref="Exception"/>.
        /// </summary>
        /// <param name="failure">The failure returned from the Runtime.</param>
        /// <returns>An <see cref="Exception"/>.</returns>
        public static Exception ToException(this Failure failure)
        {
            if (failure == null) return null;

            return Exceptions.CreateFromFailure(failure);
        }

        /// <summary>
        /// Converts a <see cref="Contracts.Failure"/> returned from the Runtime to an <see cref="Exception"/>.
        /// </summary>
        /// <param name="failure">The failure returned from the Runtime.</param>
        /// <returns>An <see cref="Exception"/>.</returns>
        public static Exception ToException(this Contracts.Failure failure)
            => failure.ToSDK().ToException();

        /// <summary>
        /// Throws an <see cref="Exception"/> if a <see cref="Contracts.Failure"/> is set.
        /// </summary>
        /// <param name="failure">The <see cref="Contracts.Failure"/> to check.</param>
        public static void ThrowIfFailureIsSet(this Contracts.Failure failure)
        {
            if (failure != null) throw failure.ToException();
        }
    }
}
