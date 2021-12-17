// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildUniqueBindings{TIdentifier,TValue,TUniqueBindings}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
public abstract class ClientUniqueBindingsBuilder<TIdentifier, TValue, TUniqueBindings> : ICanBuildUniqueBindings<TIdentifier, TValue, TUniqueBindings>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
    where TUniqueBindings : IUniqueBindings<TIdentifier, TValue>
{

    readonly Dictionary<TIdentifier, HashSet<TValue>> _identifierToValueMap = new();
    readonly Dictionary<TValue, HashSet<TIdentifier>> _valueToIdentifierMap = new();
    readonly List<ClientBuildResult> _buildResults = new();

    /// <inheritdoc />
    public virtual void Add(TIdentifier identifier, TValue value)
    {
        AddIdentifierToValueMapping(identifier, value);
        AddValueToIdentifierMapping(value, identifier);
    }

    /// <inheritdoc />
    public TUniqueBindings Build(IClientBuildResults buildResults)
    {
        foreach (var result in _buildResults)
        {
            buildResults.Add(result);
        }
        return CreateUniqueBindings(
            buildResults,
            new UniqueBindings<TIdentifier, TValue>(_identifierToValueMap
                .Where(_ => _.Value.Count == 1)
                .ToDictionary(_ => _.Key, _ => _.Value.First())));
    }

    /// <summary>
    /// Builds the <typeparamref name="TUniqueBindings"/> without having to add build results.
    /// </summary>
    /// <param name="aggregatedBuildResults">The already aggregated <see cref="IClientBuildResults"/>.</param>
    /// <param name="bindings">The unique bindings.</param>
    /// <returns>The <typeparamref name="TUniqueBindings"/>.</returns>
    protected abstract TUniqueBindings CreateUniqueBindings(IClientBuildResults aggregatedBuildResults, IUniqueBindings<TIdentifier, TValue> bindings);

    /// <summary>
    /// Adds a <see cref="ClientBuildResult"/>.
    /// </summary>
    /// <param name="buildResult">The <see cref="ClientBuildResult"/> to add.</param>
    protected void AddBuildResult(ClientBuildResult buildResult) => _buildResults.Add(buildResult);

    void AddIdentifierToValueMapping(TIdentifier identifier, TValue value)
    {
        if (_identifierToValueMap.TryGetValue(identifier, out var associatedTypes))
        {
            var newAssociatedTypes = associatedTypes.Append(value).ToHashSet();
            if (associatedTypes.Count < newAssociatedTypes.Count)
            {
                _buildResults.Add(ClientBuildResult.Failure(
                    $"{identifier} is already associated with {string.Join(", ", associatedTypes)}",
                    $"Only associate {identifier} with one {typeof(TValue)}. Maybe you are manually associating but have forgotten to turn of automatic discovery?"));
            }
        }
        else
        {
            _identifierToValueMap.Add(identifier, new HashSet<TValue> { value });
        }
    }

    void AddValueToIdentifierMapping(TValue value, TIdentifier identifier)
    {
        if (_valueToIdentifierMap.TryGetValue(value, out var associatedArtifacts))
        {
            var newAssociatedArtifacts = associatedArtifacts.Append(identifier).ToHashSet();
            if (associatedArtifacts.Count < newAssociatedArtifacts.Count)
            {
                _buildResults.Add(ClientBuildResult.Failure(
                    $"{value} is already associated with {string.Join(", ", associatedArtifacts)}",
                    $"Only associate {value} with one {nameof(TValue)}. Maybe you are manually associating but have forgotten to turn of automatic discovery?"));
            }
        }
        else
        {
            _valueToIdentifierMap.Add(value, new HashSet<TIdentifier> { identifier });
        }
    }
}
