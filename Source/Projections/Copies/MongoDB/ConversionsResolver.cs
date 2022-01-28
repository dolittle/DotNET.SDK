// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IResolveConversions"/>.
/// </summary>
public class ConversionsResolver : IResolveConversions
{
    /// <inheritdoc />
    public bool TryResolve<TProjection>(IClientBuildResults buildResults, out IDictionary<string, BsonType> conversions) where TProjection : class, new()
    {
        conversions = new Dictionary<string, BsonType>();
        return true;
    }
}
