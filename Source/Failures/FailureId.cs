// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Failures;

/// <summary>
/// Represents the failure id.
/// </summary>
public record FailureId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// The <see cref="FailureId"/> that refers to a failure that is not documented.
    /// </summary>
    public static readonly FailureId Undocumented = "05cc1d10-4efc-457b-84a6-a1be0a5f36ba";

    /// <summary>
    /// Implicitly converts the <see cref="Guid" /> to <see cref="FailureId" />.
    /// </summary>
    /// <param name="id"><see cref="Guid" /> to convert.</param>
    public static implicit operator FailureId(Guid id) => new(id);

    /// <summary>
    /// Implicitly converts the <see cref="Guid" /> to <see cref="FailureId" />.
    /// </summary>
    /// <param name="id"><see cref="Guid" /> to convert.</param>
    public static implicit operator FailureId(string id) => new(Guid.Parse(id));
}
