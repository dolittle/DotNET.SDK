// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Exception that gets thrown when an <see cref="IUniqueBindings{TIdentifier,TValue}"/> is missing a binding for a <typeparamref name="TIdentifier"/>
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
public class MissingUniqueBindingForIdentifier<TIdentifier, TValue> : Exception
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
{
    /// <summary>
    /// Initializes an instance of the <see cref="MissingUniqueBindingForIdentifier{TIdentifier,TValue}"/>. 
    /// </summary>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/> unique identifier that is missing a binding.</param>
    public MissingUniqueBindingForIdentifier(TIdentifier identifier)
        : base($"Missing a unique binding for identifier {identifier} of type {typeof(TIdentifier)} to a value of type {typeof(TValue)}")
    {
    }
}
