// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// The Dolittle Mongo Conventions.
/// </summary>
public static class DolittleMongoConventions
{
    static readonly object _registerLock = new();
    static bool _isRegistered;
    
    /// <summary>
    /// The name of the Dolittle conventions pack.
    /// </summary>
    public const string ConventionPackName = "Dolittle Conventions";
    
    /// <summary>
    /// Ensured that the default Mongo Conventions are registered;
    /// </summary>
    public static void EnsureConventionsAreRegistered(GuidRepresentation defaultRepresentation)
    {
        if (_isRegistered)
        {
            return;
        }
        lock (_registerLock)
        {
            if (_isRegistered)
            {
                return;
            }
            _isRegistered = true;
            BsonSerializer.RegisterSerializationProvider(new ConceptSerializationProvider());
            if (defaultRepresentation != GuidRepresentation.Unspecified)
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(defaultRepresentation));
            }
            var pack = new ConventionPack();
            pack.AddClassMapConvention("Ignore extra elements", _ => _.SetIgnoreExtraElements(true));
            ConventionRegistry.Register(ConventionPackName, pack,  _ => true);
        }
    }
}
