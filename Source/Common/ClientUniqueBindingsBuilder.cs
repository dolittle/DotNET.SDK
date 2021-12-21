// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildUniqueBindings{TIdentifier,TValue}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
public class ClientUniqueBindingsBuilder<TIdentifier, TValue> : ICanBuildUniqueBindings<TIdentifier, TValue>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
{
    readonly Dictionary<TIdentifier, HashSet<TValue>> _identifierToValueMap = new();
    readonly Dictionary<TValue, HashSet<TIdentifier>> _valueToIdentifierMap = new();
    readonly List<ClientBuildResult> _buildResults = new();

    /// <summary>
    /// Initializes an instance of the <see cref="ClientUniqueBindingsBuilder{TIdentifier,TValue}"/>.
    /// </summary>
    /// <param name="identifierLabel">The label of the identifier. Used for <see cref="IClientBuildResults"/>.</param>
    /// <param name="valueLabel">The label of the value. Used for <see cref="IClientBuildResults"/>.</param>
    public ClientUniqueBindingsBuilder(string identifierLabel = nameof(TIdentifier), string valueLabel = nameof(TValue))
    {
        IdentifierLabel = identifierLabel;
        ValueLabel = valueLabel;
    }

    /// <summary>
    /// Gets the descriptive label of <typeparamref name="TIdentifier"/>.
    /// </summary>
    public string IdentifierLabel { get; }
    
    /// <summary>
    /// Gets the descriptive label of <typeparamref name="TValue"/>.
    /// </summary>
    public string ValueLabel { get; }
    
    /// <inheritdoc />
    public virtual void Add(TIdentifier identifier, TValue value)
    {
        AddIdentifierToValueMapping(identifier, value);
        AddValueToIdentifierMapping(value, identifier);
    }

    /// <inheritdoc />
    public IUniqueBindings<TIdentifier, TValue> Build(IClientBuildResults buildResults)
    {
        var idToValueExclusions = GetKeysAndValuesToExclude(_identifierToValueMap, IdentifierLabel, ValueLabel);
        var valueToIdExclusions = GetKeysAndValuesToExclude(_valueToIdentifierMap, ValueLabel, IdentifierLabel);
        var allExclusions = new ExclusionResult<TIdentifier, TValue>(
            idToValueExclusions.ExcludedKeys.Concat(valueToIdExclusions.ExcludedValues).ToHashSet(),
            idToValueExclusions.ExcludedValues.Concat(valueToIdExclusions.ExcludedKeys).ToHashSet(),
            idToValueExclusions.BuildResults.Concat(valueToIdExclusions.BuildResults).ToArray());
        foreach (var result in _buildResults.Concat(allExclusions.BuildResults))
        {
            buildResults.Add(result);
        }

        return new UniqueBindings<TIdentifier, TValue>(_identifierToValueMap
            .Where(_ => HasOnlyOneBinding(_) && IsNotExcluded(_, allExclusions))
            .ToDictionary(_ => _.Key, _ => _.Value.First()));
    }

    /// <summary>
    /// Adds a <see cref="ClientBuildResult"/>.
    /// </summary>
    /// <param name="buildResult">The <see cref="ClientBuildResult"/> to add.</param>
    protected void AddBuildResult(ClientBuildResult buildResult) => _buildResults.Add(buildResult);

    static ExclusionResult<TKey, TVal> GetKeysAndValuesToExclude<TKey, TVal>(
        Dictionary<TKey, HashSet<TVal>> items,
        string keyLabel,
        string valueLabel)
    {
        var excludedKeys = new HashSet<TKey>();
        var excludedValues = new HashSet<TVal>();
        var buildResults = new List<ClientBuildResult>();
        foreach (var (key, values) in items)
        {
            if (values.Count <= 1)
            {
                continue;
            }
            buildResults.Add(ClientBuildResult.Failure(
                $"{keyLabel} {key} and all {valueLabel} bound to it will not be built because it is bound to multiple {valueLabel} values: {string.Join(", ", values)}",
                $"{keyLabel} {key} must only be bound to one {valueLabel}"));
            
            foreach (var value in values)
            {
                excludedValues.Add(value);
            }
            excludedKeys.Add(key);
        } 
        return new ExclusionResult<TKey, TVal>(excludedKeys, excludedValues, buildResults.ToArray());
    }

    static bool HasOnlyOneBinding<TKey, TVal>(KeyValuePair<TKey, HashSet<TVal>> kvp)
        => kvp.Value.Count == 1;

    static bool IsNotExcluded<TKey, TVal>(KeyValuePair<TKey, HashSet<TVal>> kvp, ExclusionResult<TKey, TVal> toExclude)
        => !toExclude.ExcludedKeys.Contains(kvp.Key) && toExclude.ExcludedValues.Contains(kvp.Value.FirstOrDefault());

    void AddIdentifierToValueMapping(TIdentifier identifier, TValue value)
    {
        if (_identifierToValueMap.TryGetValue(identifier, out var associateValues))
        {
            associateValues.Add(value);
        }
        else
        {
            _identifierToValueMap.Add(identifier, new HashSet<TValue> { value });
        }
    }

    void AddValueToIdentifierMapping(TValue value, TIdentifier identifier)
    {
        if (_valueToIdentifierMap.TryGetValue(value, out var associatedIdentifiers))
        {
            associatedIdentifiers.Add(identifier);
        }
        else
        {
            _valueToIdentifierMap.Add(value, new HashSet<TIdentifier> { identifier });
        }
    }

    class ExclusionResult<TKey, TVal>
    {
        public ExclusionResult(HashSet<TKey> excludedKeys, HashSet<TVal> excludedValues, ClientBuildResult[] buildResults)
        {
            ExcludedKeys = excludedKeys;
            ExcludedValues = excludedValues;
            BuildResults = buildResults;
        }
        public HashSet<TKey> ExcludedKeys { get; }
        public HashSet<TVal> ExcludedValues { get; }
        public ClientBuildResult[] BuildResults { get; }
    }
}
