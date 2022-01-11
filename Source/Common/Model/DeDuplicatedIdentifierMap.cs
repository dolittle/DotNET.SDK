// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents an <see cref="IdentifierMap{TValue}"/> with no duplicates
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class DeDuplicatedIdentifierMap<TValue> : IdentifierMap<TValue>
{
    /// <summary>
    /// Initializes an instance of the <see cref="DeDuplicatedIdentifierMap{TValue}"/> 
    /// </summary>
    /// <param name="map">The <see cref="IdentifierMap{TValue}"/> with potential duplicates.</param>
    /// <param name="onDuplicate">The <see cref="OnDuplicateBindingsCallback{TValue}"/> to perform on duplicated bindings.</param>
    public DeDuplicatedIdentifierMap(IdentifierMap<TValue> map, OnDuplicateBindingsCallback<TValue> onDuplicate = default)
    {
        foreach (var (identifierId, bindings) in map)
        {
            var groupedBindings = bindings.GroupBy(binding => binding).ToArray();
            PerformCallbackOnDuplicateBindings(groupedBindings, onDuplicate);
            Add(identifierId, groupedBindings.Where(ThereAreNoDuplicates).Select(_ => _.Key).ToList());
        }
    }

    static void PerformCallbackOnDuplicateBindings(IEnumerable<IGrouping<IdentifierMapBinding<TValue>, IdentifierMapBinding<TValue>>> groupedBindings, OnDuplicateBindingsCallback<TValue> callback = default)
    {
        foreach (var duplicate in groupedBindings
                     .Where(ThereAreDuplicates)
                     .Select(group => new {binding = group.Key, count = group.Count()}))
        {
            callback?.Invoke(duplicate.binding.Binding, duplicate.binding.BindingValue, duplicate.count);
        }
    }

    static bool ThereAreDuplicates(IGrouping<IdentifierMapBinding<TValue>, IdentifierMapBinding<TValue>> bindings) => bindings.Count() > 1;
    static bool ThereAreNoDuplicates(IGrouping<IdentifierMapBinding<TValue>, IdentifierMapBinding<TValue>> bindings) => !ThereAreDuplicates(bindings);
}
