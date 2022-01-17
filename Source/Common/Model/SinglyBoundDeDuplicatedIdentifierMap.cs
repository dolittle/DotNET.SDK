// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents an <see cref="IdentifierMap{TValue}"/> with no duplicates
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class SinglyBoundDeDuplicatedIdentifierMap<TValue> : IdentifierMap<TValue>
{
    record ValueToIdentifiersMap(TValue BindingValue, IEnumerable<IBinding> Identifiers);
    
    /// <summary>
    /// Initializes an instance of the <see cref="DeDuplicatedIdentifierMap{TValue}"/> 
    /// </summary>
    /// <param name="map">The <see cref="IdentifierMap{TValue}"/> with potential duplicates.</param>
    /// <param name="onBindingValueBoundToMultipleIdentifiers">The <see cref="OnBindingValueBoundToMultipleIdentifiers{TValue}"/> to perform on duplicated bindings.</param>
    public SinglyBoundDeDuplicatedIdentifierMap(DeDuplicatedIdentifierMap<TValue> map, OnBindingValueBoundToMultipleIdentifiers<TValue> onBindingValueBoundToMultipleIdentifiers = default)
    {
        var grouped = map
            .Values
            .SelectMany(mappedBindings=> mappedBindings)
            .GroupBy(_ => _.BindingValue, (value, bindings) => new ValueToIdentifiersMap(value, bindings.Select(_ => _.Binding)));
        foreach (var (bindingValue, groupedBindings) in grouped.Select(_ => (_.BindingValue, _.Identifiers.ToArray())))
        {
            var identifiers = groupedBindings.Select(_ => _.Identifier).ToArray();
            if (identifiers.Length > 1)
            {
                onBindingValueBoundToMultipleIdentifiers?.Invoke(bindingValue, identifiers);
            }
            else
            {
                AddBinding(groupedBindings[0], bindingValue);
            }
        }
    }
}
