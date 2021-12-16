// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="UniqueBindings{TIdentifier,TValue}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
public class UniqueBindings<TIdentifier, TValue> : IUniqueBindings<TIdentifier, TValue>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
{
    readonly IDictionary<TValue, TIdentifier> _valueToIdentifier;

    /// <summary>
    /// Initializes an instance of the <see cref="UniqueBindings{TIdentifier,TValue}"/> class.
    /// </summary>
    /// <param name="bindings"></param>
    public UniqueBindings(IDictionary<TIdentifier, TValue> bindings)
    {
        Bindings = bindings.ToDictionary(_ => _.Key, _ => _.Value);
        _valueToIdentifier = bindings.ToDictionary(_ => _.Value, _ => _.Key);
    }

    /// <summary>
    /// Initializes an instance of the <see cref="UniqueBindings{TIdentifier,TValue}"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/>.</param>
    public UniqueBindings(IUniqueBindings<TIdentifier, TValue> bindings)
    {
        Bindings = bindings.Bindings;
    }

    /// <inheritdoc />
    public IDictionary<TIdentifier, TValue> Bindings { get; }

    /// <inheritdoc />
    public IEnumerable<TIdentifier> Identifiers => Bindings.Keys;

    /// <inheritdoc />
    public IEnumerable<TValue> Values => _valueToIdentifier.Keys;

    /// <inheritdoc />
    public bool HasFor(TIdentifier identifier)
        => Bindings.ContainsKey(identifier);

    /// <inheritdoc />
    public bool HasFor(TValue value)
        => _valueToIdentifier.ContainsKey(value);

    /// <inheritdoc />
    public TValue GetFor(TIdentifier identifier)
        => Bindings.TryGetValue(identifier, out var value)
            ? value
            : throw new MissingUniqueBindingForIdentifier<TIdentifier, TValue>(identifier);

    /// <inheritdoc />
    public TIdentifier GetFor(TValue value)
        => _valueToIdentifier.TryGetValue(value, out var identifier)
            ? identifier
            : throw new MissingUniqueBindingForValue<TIdentifier, TValue>(value);
}
