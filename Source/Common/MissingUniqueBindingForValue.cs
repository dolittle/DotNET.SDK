// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Exception that gets thrown when an <see cref="IUniqueBindings{TIdentifier,TValue}"/> is missing a binding for a <typeparamref name="TValue"/>
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
public class MissingUniqueBindingForValue<TIdentifier, TValue> : Exception
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
{
    /// <summary>
    /// Initializes an instance of the <see cref="MissingUniqueBindingForValue{TIdentifier,TValue}"/>. 
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/> unique identifier that is missing a binding.</param>
    public MissingUniqueBindingForValue(TValue value)
        : base($"Missing a unique binding for value {value} of type {typeof(TValue)} to an identifier of type {typeof(TIdentifier)}")
    {
    }
}
