// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Exception that gets thrown when there are multiple keys associated to a value.
/// </summary>
/// <typeparam name="TKey">The <see cref="Type" /> of the unique key.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the unique value to associate with the unique key.</typeparam>
public class CannotHaveMultipleKeysAssociatedWithValue<TKey, TValue> : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="CannotHaveMultipleKeysAssociatedWithValue{TKey,TValue}"/> class.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="key"></param>
    /// <param name="existingKey"></param>
    public CannotHaveMultipleKeysAssociatedWithValue(TValue value, TKey key, TKey existingKey)
        : base($"Key {key} cannot be associated with value {value} because it is already associated with {existingKey}")
    {
    }
}
