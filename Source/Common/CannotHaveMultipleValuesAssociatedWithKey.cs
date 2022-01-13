// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Exception that gets thrown when there are multiple values associated to a key.
/// </summary>
/// <typeparam name="TKey">The <see cref="Type" /> of the unique key.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the unique value to associate with the unique key.</typeparam>
public class CannotHaveMultipleValuesAssociatedWithKey<TKey, TValue> : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="CannotHaveMultipleValuesAssociatedWithKey{TKey,TValue}"/> class.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="existingValue"></param>
    public CannotHaveMultipleValuesAssociatedWithKey(TKey key, TValue value, TValue existingValue)
        : base($"Value {value} of type {value.GetType()} cannot be associated with key {key} of type {key.GetType()} because it is already associated with {existingValue}")
    {
    }
}
