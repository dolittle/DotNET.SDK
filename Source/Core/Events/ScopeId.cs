// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the identification of a scope.
/// </summary>
public record ScopeId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// The <see cref="ScopeId"/> that refers to the default scope.
    /// </summary>
    public static readonly ScopeId Default = Guid.Empty;

    /// <summary>
    /// Implicitly convert from a <see cref="Guid"/> to an <see cref="ScopeId"/>.
    /// </summary>
    /// <param name="scopeId">ScopeId as <see cref="Guid"/>.</param>
    public static implicit operator ScopeId(Guid scopeId) => new(scopeId);

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="ScopeId"/>.
    /// </summary>
    /// <param name="scopeId">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="ScopeId"/>.</returns>
    public static implicit operator ScopeId(string scopeId) => new(Guid.Parse(scopeId));
}
