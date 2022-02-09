// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// The Dolittle Mongo Conventions.
/// </summary>
public static class DolittleMongoConventions
{
    static readonly object _registerLock = new();
    static bool _isRegistered;
    
    /// <summary>
    /// Ensured that the default Mongo Conventions are registered;
    /// </summary>
    public static void EnsureConventionsAreRegistered()
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
            var pack = new ConventionPack();
            pack.AddClassMapConvention("Ignore extra elements", _ => _.SetIgnoreExtraElements(true));
            ConventionRegistry.Register("Dolittle Conventions", pack,  _ => true);
        }
    }
}
