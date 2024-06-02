// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Testing.Projections;

/// <summary>
/// Asserts against a collection of projections.
/// </summary>
/// <typeparam name="TProjection"></typeparam>
public class ProjectionAssertions<TProjection>
    where TProjection : ReadModel, new()
{
    readonly ImmutableDictionary<Key, TProjection> _projections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionAssertions{TProjection}"/> class.
    /// </summary>
    /// <param name="projections"></param>
    public ProjectionAssertions(IDictionary<Key, TProjection> projections)
    {
        _projections = projections.ToImmutableDictionary();
    }

    /// <summary>
    /// Get the read model for a specific key, or throw if it does not exist.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ReadModelDidNotExist"></exception>
    public TProjection ReadModel(Key key)
    {
        return _projections.GetValueOrDefault(key) ?? throw new ReadModelDidNotExist(key);
    }

    /// <summary>
    /// Get the read model assertions for a specific key, or throw if it does not exist.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ReadModelDidNotExist"></exception>
    public ReadModelValueAssertion<TProjection> HasReadModel(Key key)
    {
        return new ReadModelValueAssertion<TProjection>(ReadModel(key));
    }

    /// <summary>
    /// Assert that a read model for a specific key does not exist, or throw if it does.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="ReadModelExistedWhenItShouldNot"></exception>
    public void ReadModelDoesNotExist(Key key)
    {
        if (_projections.TryGetValue(key, out var projection))
        {
            throw new ReadModelExistedWhenItShouldNot(key, projection);
        }
    }
}

/// <summary>
/// Exception that gets thrown when a read model exists when it should not.
/// </summary>
public class ReadModelExistedWhenItShouldNot : DolittleAssertionFailed
{
    /// <inheritdoc />
    public ReadModelExistedWhenItShouldNot(Key key, object projection)
        : base($"Read model for {key} existed when it should not. Projection: {projection}")
    {
    }
}

/// <summary>
/// Exception that gets thrown when a read model does not exist.
/// </summary>
public class ReadModelDidNotExist : DolittleAssertionFailed
{
    /// <inheritdoc />
    public ReadModelDidNotExist(Key key)
        : base($"Read model for {key} did not exist")
    {
    }
}
