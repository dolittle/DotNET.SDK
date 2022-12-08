// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using PbFailure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.SDK.Failures;

/// <summary>
/// Extension methods for <see cref="Failure"/>.
/// </summary>
static class FailureExtensions
{
    /// <summary>
    /// Converts <see cref="PbFailure"/> to a <see cref="Failure"/>.
    /// </summary>
    /// <param name="failure"><see cref="PbFailure" /> to convert.</param>
    /// <returns>The converted <see cref="Failure"/>.</returns>
    public static Failure? ToSDK(this PbFailure? failure)
    {
        if (failure is null) return null;

        return !failure.Id.TryTo<FailureId>(out var id, out _)
            ? new Failure(FailureId.Undocumented, failure.Reason)
            : new Failure(id, failure.Reason);
    }

    /// <summary>
    /// Converts a <see cref="Failure"/> returned from the Runtime to an <see cref="Exception"/>.
    /// </summary>
    /// <param name="failure">The failure returned from the Runtime.</param>
    /// <returns>An <see cref="Exception"/>.</returns>
    static Exception ToException(this Failure failure) => Exceptions.CreateFromFailure(failure);

    /// <summary>
    /// Converts a <see cref="PbFailure"/> returned from the Runtime to an <see cref="Exception"/>.
    /// </summary>
    /// <param name="failure">The failure returned from the Runtime.</param>
    /// <returns>An <see cref="Exception"/>.</returns>
    static Exception ToException(this PbFailure failure)
        => failure.ToSDK().ToException();

    /// <summary>
    /// Throws an <see cref="Exception"/> if a <see cref="PbFailure"/> is set.
    /// </summary>
    /// <param name="failure">The <see cref="PbFailure"/> to check.</param>
    public static void ThrowIfFailureIsSet(this PbFailure? failure)
    {
        if (failure != null)
        {
            throw failure.ToException();
        }
    }
}
