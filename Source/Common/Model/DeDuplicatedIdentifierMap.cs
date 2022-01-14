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
public class DeDuplicatedIdentifierMap<TValue> : IdentifierMap<TValue>
{
    record CountedBindings<TValue>(IdentifierMapBinding<TValue> Binding, int NumBindings);
    /// <summary>
    /// Initializes an instance of the <see cref="DeDuplicatedIdentifierMap{TValue}"/> 
    /// </summary>
    /// <param name="map">The <see cref="IdentifierMap{TValue}"/> with potential duplicates.</param>
    /// <param name="onDuplicate">The <see cref="OnDuplicateBindingsCallback{TValue}"/> to perform on duplicated bindings.</param>
    public DeDuplicatedIdentifierMap(IdentifierMap<TValue> map, OnDuplicateBindingsCallback<TValue> onDuplicate = default)
    {
        foreach (var (identifierId, bindings) in map)
        {
            var countedBindings = bindings.GroupBy(binding => binding, (binding, bindings) => new CountedBindings<TValue>(binding, bindings.Count())).ToArray();
            PerformCallbackOnDuplicateBindings(countedBindings, onDuplicate);
            Add(identifierId, countedBindings.Select(_ => _.Binding).ToList());
        }
    }

    static void PerformCallbackOnDuplicateBindings(IEnumerable<CountedBindings<TValue>> countedBindings, OnDuplicateBindingsCallback<TValue> callback = default)
    {
        foreach (var ((binding, value), numDuplicates) in countedBindings.Where(_ => _.NumBindings > 1))
        {
            Console.WriteLine($"Performing duplicate callback for {binding} with {numDuplicates} duplicates");
            callback?.Invoke(binding, value, numDuplicates);
        }
    }
}
