// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Testing.Projections;

public class ProjectionAssertions<TProjection>
    where TProjection : ReadModel, new()
{
    readonly ImmutableDictionary<Key, TProjection> _projections;


    public ProjectionAssertions(IDictionary<Key, TProjection> projections)
    {
        _projections = projections.ToImmutableDictionary();
    }

    public TProjection ReadModel(Key key)
    {
        return _projections.GetValueOrDefault(key) ?? throw new ReadModelDidNotExist(key);
    }

    public ReadModelValueAssertion<TProjection> HasReadModel(Key key)
    {
        return new ReadModelValueAssertion<TProjection>(ReadModel(key));
    }

    public void ReadModelDoesNotExist(Key key)
    {
        if (_projections.TryGetValue(key, out var projection))
        {
            throw new ReadModelExistedWhenItShouldNot(key, projection);
        }
    }
}

public class ReadModelExistedWhenItShouldNot : DolittleAssertionFailed
{
    public ReadModelExistedWhenItShouldNot(Key key, object projection)
        : base($"Read model for {key} existed when it should not. Projection: {projection}")
    {
    }
}

public class ReadModelDidNotExist : DolittleAssertionFailed
{
    public ReadModelDidNotExist(Key key)
        : base($"Read model for {key} did not exist")
    {
    }
}
