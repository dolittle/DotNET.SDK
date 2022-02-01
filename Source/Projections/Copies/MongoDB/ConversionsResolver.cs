// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IGetDefaultConversionsFromReadModel"/>.
/// </summary>
public class ConversionsResolver : IGetDefaultConversionsFromReadModel
{
    /// <inheritdoc />
    public bool TryGetFrom<TProjection>(IClientBuildResults buildResults, out IDictionary<string, BsonType> conversions)
        where TProjection : class, new()
    {
        throw new NotImplementedException();
    }
}
