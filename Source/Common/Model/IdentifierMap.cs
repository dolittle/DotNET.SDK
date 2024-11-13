// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents a mapping between a globally unique identifier and a list of <see cref="IIdentifier"/> and <typeparamref name="TValue"/> pairs.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class IdentifierMap<TValue> : Dictionary<Guid, List<IdentifierMapBinding<TValue>>>
{
    /// <summary>
    /// Adds an <see cref="IBinding"/> to a <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="binding">The <see cref="IBinding"/> to add.</param>
    /// <param name="bindingValue">The value that is bound.</param>
    public void AddBinding(IBinding binding, TValue bindingValue)
    {
        if (!TryGetValue(binding.Identifier.Id, out var bindings))
        {
            bindings = [];
        }
        bindings.Add(new IdentifierMapBinding<TValue>(binding, bindingValue));
        this[binding.Identifier.Id] = bindings;
    }
}
