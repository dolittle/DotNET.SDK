// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Defines an identifier in an application model.
/// </summary>
/// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
public interface IIdentifier<out TId> : IIdentifier
    where TId : ConceptAs<Guid>
{
    /// <summary>
    /// Gets the globally unique id for the identifier.
    /// </summary>
    new TId Id { get; }
    
    /// <summary>
    /// Determines whether or not this identifier can coexist with another identifier.
    /// By default identifiers with a similar id cannot coexist, but subtype can implement custom logic.
    /// </summary>
    /// <param name="identifier">The other identifier to check if can coexist with.</param>
    /// <returns>True if this identifier can coexist with the other identifier, false if not.</returns>
    public bool CanCoexistWith(IIdentifier<ConceptAs<Guid>> identifier);
}

/// <summary>
/// Defines an identifier in an application model.
/// </summary>
public interface IIdentifier
{
    /// <summary>
    /// Gets the globally unique id for the identifier.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Determines whether or not this identifier can coexist with another identifier.
    /// By default identifiers with a similar id cannot coexist, but subtype can implement custom logic.
    /// </summary>
    /// <param name="identifier">The other identifier to check if can coexist with.</param>
    /// <returns>True if this identifier can coexist with the other identifier, false if not.</returns>
    bool CanCoexistWith(IIdentifier identifier);
}
