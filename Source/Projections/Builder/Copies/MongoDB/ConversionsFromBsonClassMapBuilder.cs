// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildPropertyConversionsFromReadModel"/>.
/// </summary>
public class ConversionsFromBsonClassMapBuilder : IBuildPropertyConversionsFromBsonClassMap
{
    readonly IAddConversionsFromBsonClassMap _conversionsFromBsonClassMap;
    
    /// <summary>
    /// Initializes a new instance of <see cref="ConversionsFromBsonClassMapBuilder"/> class.
    /// </summary>
    /// <param name="conversionsFromBsonClassMap">The <see cref="IAddConversionsFromBsonClassMap" />.</param>
    public ConversionsFromBsonClassMapBuilder(IAddConversionsFromBsonClassMap conversionsFromBsonClassMap)
    {
        _conversionsFromBsonClassMap = conversionsFromBsonClassMap;
    }
    
    /// <inheritdoc />
    public void BuildFrom<TReadModel>(IClientBuildResults buildResults, IPropertyConversions conversions)
        where TReadModel : class, new()
        => _conversionsFromBsonClassMap.AddFrom(BsonClassMap.LookupClassMap(typeof(TReadModel)), buildResults, conversions);
}
