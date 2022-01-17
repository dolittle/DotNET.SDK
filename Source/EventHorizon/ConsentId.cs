// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Represents the concept of a unique identifier for a consent from an event horizon subscription.
/// </summary>
public record ConsentId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Gets the consent id used if it is not set.
    /// </summary>
    public static ConsentId NotSet => new(Guid.Empty);

    /// <summary>
    /// Implicitly converts from a <see cref="Guid"/> to an <see cref="ConsentId"/>.
    /// </summary>
    /// <param name="consentId">The <see cref="Guid"/> representation.</param>
    /// <returns>The converted <see cref="ConsentId"/>.</returns>
    public static implicit operator ConsentId(Guid consentId) => new(consentId);

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="ConsentId"/>.
    /// </summary>
    /// <param name="consentId">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="ConsentId"/>.</returns>
    public static implicit operator ConsentId(string consentId) => new(Guid.Parse(consentId));
}