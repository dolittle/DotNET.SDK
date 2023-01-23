// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Resources.Contracts;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Dolittle.SDK.Resources.MongoDB.Internal;

/// <summary>
/// Represents a client for <see cref="Runtime.Resources.Contracts.Resources"/> and an implementation of <see cref="IResources"/>.
/// </summary>
public class MongoDBResource : IMongoDBResource
{
    readonly MongoUrl _mongoUrl;
    readonly MongoClient _mongoClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBResource"/> class.
    /// </summary>
    /// <param name="runtimeMongoDBResponse">The MongoDB resource response from the Runtime.</param>
    public MongoDBResource(GetMongoDBResponse runtimeMongoDBResponse)
    {
        _mongoUrl = MongoUrl.Create(runtimeMongoDBResponse.ConnectionString);
        var clientSettings = MongoClientSettings.FromUrl(_mongoUrl);
        clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        _mongoClient = new MongoClient(clientSettings.Freeze());
    }

    /// <inheritdoc />
    public IMongoDatabase GetDatabase(Action<MongoDatabaseSettings>? databaseSettingsCallback = default)
    {
        DolittleMongoConventions.EnsureConventionsAreRegistered();
        var databaseSettings = new MongoDatabaseSettings();
        databaseSettingsCallback?.Invoke(databaseSettings);
        return _mongoClient.GetDatabase(_mongoUrl.DatabaseName, databaseSettings);
    }
}
