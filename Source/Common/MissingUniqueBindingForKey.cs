// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Exception that gets thrown when an <see cref="IUniqueBindings{Tkey,TValue}"/> is missing a binding for a <typeparamref name="TKey"/>
/// </summary>
/// <typeparam name="TKey">The <see cref="Type" /> of the unique key.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the unique value to associate with the unique key.</typeparam>
public class MissingUniqueBindingForKey<TKey, TValue> : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="MissingUniqueBindingForKey{TIdentifier,TValue}"/>. 
    /// </summary>
    /// <param name="key">The <typeparamref name="TKey"/> unique identifier that is missing a binding.</param>
    public MissingUniqueBindingForKey(TKey key)
        : base($"Missing a unique binding for key {key} of type {typeof(TKey)} to a value of type {typeof(TValue)}")
    {
    }
}
