// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Common;

/// <summary>
/// Defines a collection of unique bindings
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
public interface IUniqueBindings<TIdentifier, TValue>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
{
    /// <summary>
    /// Gets all the <typeparamref name="TIdentifier"/> to <typeparamref name="TValue"/> bindings.
    /// </summary>
    IDictionary<TIdentifier, TValue> Bindings { get; }

    /// <summary>
    /// Gets all the <typeparamref name="TIdentifier"/>.
    /// </summary>
    IEnumerable<TIdentifier> Identifiers { get; }
        
    /// <summary>
    /// Gets all the <typeparamref name="TValue"/>.
    /// </summary>
    IEnumerable<TValue> Values { get; }

    /// <summary>
    /// Check if there is binding for <typeparamref name="TIdentifier"/>.
    /// </summary>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/>.</param>
    /// <returns><see cref="bool"/>.</returns>
    bool HasFor(TIdentifier identifier);
    
    /// <summary>
    /// Check if there is binding for <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/>.</param>
    /// <returns><see cref="bool"/>.</returns>
    bool HasFor(TValue value);

    /// <summary>
    /// Gets the <typeparamref name="TValue"/> bound to the <typeparamref name="TIdentifier"/>.
    /// </summary>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/>.</param>
    /// <returns><see cref="TValue"/>.</returns>
    TValue GetFor(TIdentifier identifier);
    
    /// <summary>
    /// Gets the <typeparamref name="TIdentifier"/> bound to the <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/>.</param>
    /// <returns><see cref="TIdentifier"/>.</returns>
    TIdentifier GetFor(TValue value);
}
