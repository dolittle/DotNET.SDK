// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Common;

/// <summary>
/// Defines a read-only collection of unique bindings where each <typeparamref name="TKey"/> and <typeparamref name="TValue"/> is unique.
/// </summary>
/// <typeparam name="TKey">The <see cref="Type" /> of the unique key.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the unique value to associate with the unique key.</typeparam>
public interface IUniqueBindings<TKey, TValue>
{
    /// <summary>
    /// Gets all the <typeparamref name="TKey"/> and <typeparamref name="TValue"/> bindings.
    /// </summary>
    IEnumerable<(TKey, TValue)> Bindings { get; }

    /// <summary>
    /// Gets all the <typeparamref name="TKey"/>.
    /// </summary>
    IEnumerable<TKey> Keys { get; }
        
    /// <summary>
    /// Gets all the <typeparamref name="TValue"/>.
    /// </summary>
    IEnumerable<TValue> Values { get; }

    /// <summary>
    /// Check if there is binding for <typeparamref name="TKey"/>.
    /// </summary>
    /// <param name="key">The <typeparamref name="TKey"/>.</param>
    /// <returns><see cref="bool"/>.</returns>
    bool HasFor(TKey key);
    
    /// <summary>
    /// Check if there is binding for <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/>.</param>
    /// <returns><see cref="bool"/>.</returns>
    bool HasFor(TValue value);

    /// <summary>
    /// Gets the <typeparamref name="TValue"/> bound to the <typeparamref name="TKey"/>.
    /// </summary>
    /// <param name="key">The <typeparamref name="TKey"/>.</param>
    /// <returns><typeparamref name="TValue"/>.</returns>
    TValue GetFor(TKey key);
    
    /// <summary>
    /// Gets the <typeparamref name="TKey"/> bound to the <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/>.</param>
    /// <returns><typeparamref name="TKey"/>.</returns>
    TKey GetFor(TValue value);
}
