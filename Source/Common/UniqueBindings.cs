// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="IUniqueBindings{TKey,TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The <see cref="Type" /> of the unique key.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the unique value to associate with the unique key.</typeparam>
public class UniqueBindings<TKey, TValue> : IUniqueBindings<TKey, TValue>
{
    readonly Dictionary<TValue, TKey> _valueToKey = new();
    readonly Dictionary<TKey, TValue> _keyToValue = new();

    /// <summary>
    /// Initializes an instance of the <see cref="UniqueBindings{TIdentifier,TValue}"/> class.
    /// </summary>
    /// <param name="bindings"></param>
    public UniqueBindings(IDictionary<TKey, TValue> bindings)
    {
        foreach (var binding in bindings)
        {
            Add(binding);
        }
    }

    /// <summary>
    /// Initializes an instance of the <see cref="UniqueBindings{TIdentifier,TValue}"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/>.</param>
    public UniqueBindings(IUniqueBindings<TKey, TValue> bindings)
    {
        foreach (var (key, value) in bindings.Bindings)
        {
            AddBinding(key, value);
        }
    }

    /// <summary>
    /// Initializes an instance of the <see cref="UniqueBindings{TKey,TValue}"/> class.
    /// </summary>
    public UniqueBindings()
    {
    }

    /// <inheritdoc />
    public IEnumerable<(TKey, TValue)> Bindings => _keyToValue.Select(_ => (_.Key, _.Value));

    /// <inheritdoc />
    public IEnumerable<TKey> Keys => _keyToValue.Keys;

    /// <inheritdoc />
    public IEnumerable<TValue> Values => _valueToKey.Keys;

    /// <inheritdoc />
    public bool HasFor(TKey key)
        => _keyToValue.ContainsKey(key);

    /// <inheritdoc />
    public bool HasFor(TValue value)
        => _valueToKey.ContainsKey(value);

    /// <inheritdoc />
    public TValue GetFor(TKey key)
        => _keyToValue.TryGetValue(key, out var value)
            ? value
            : throw new MissingUniqueBindingForKey<TKey, TValue>(key);

    /// <inheritdoc />
    public TKey GetFor(TValue value)
        => _valueToKey.TryGetValue(value, out var identifier)
            ? identifier
            : throw new MissingUniqueBindingForValue<TKey, TValue>(value);

    /// <summary>
    /// Adds a <typeparamref name="TKey"/> to <typeparamref name="TValue"/> binding.
    /// </summary>
    /// <param name="key">The unique key.</param>
    /// <param name="value">The unique value.</param>
    public void Add(TKey key, TValue value)
    {
        ThrowIfValueAlreadyAssociatedWithKey(value, key);
        ThrowIfKeyAlreadyAssociatedWithValue(key, value);

        AddBinding(key, value);
    }

    /// <summary>
    /// Adds a <typeparamref name="TKey"/> to <typeparamref name="TValue"/> binding.
    /// </summary>
    /// <param name="binding">The unique <typeparamref name="TKey"/> to <typeparamref name="TValue"/> binding.</param>
    public void Add((TKey, TValue) binding) => Add(binding.Item1, binding.Item2);
    
    /// <summary>
    /// Adds a <typeparamref name="TKey"/> to <typeparamref name="TValue"/> binding.
    /// </summary>
    /// <param name="binding">The unique <typeparamref name="TKey"/> to <typeparamref name="TValue"/> binding.</param>
    public void Add(KeyValuePair<TKey, TValue> binding) => Add(binding.Key, binding.Value);

    void AddBinding(TKey key, TValue value)
    {
        _valueToKey.Add(value, key);
        _keyToValue.Add(key, value);
    }

    void ThrowIfKeyAlreadyAssociatedWithValue(TKey key, TValue value)
    {
        if (_keyToValue.TryGetValue(key, out var existingValue))
        {
            throw new CannotHaveMultipleValuesAssociatedWithKey<TKey, TValue>(key, value, existingValue);
        }
    }

    void ThrowIfValueAlreadyAssociatedWithKey(TValue value, TKey key)
    {
        if (_valueToKey.TryGetValue(value, out var existingKey))
        {
            throw new CannotHaveMultipleKeysAssociatedWithValue<TKey, TValue>(value, key, existingKey);
        }
    }
}
