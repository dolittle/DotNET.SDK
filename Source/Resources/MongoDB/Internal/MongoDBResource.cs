// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Resources.Contracts;
using MongoDB.Driver;

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
    /// <param name="runtimeMongoDbResponse">The MongoDB resource response from the Runtime.</param>
    /// <param name="clientSettingsCallback"></param>
    public MongoDBResource(GetMongoDBResponse runtimeMongoDbResponse,
        Action<MongoClientSettings>? clientSettingsCallback)
    {
        _mongoUrl = MongoUrl.Create(runtimeMongoDbResponse.ConnectionString);
        var clientSettings = MongoClientSettings.FromUrl(_mongoUrl);
        ConfigureSettings(clientSettingsCallback, clientSettings);

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

    /// <summary>
    /// Configures <see cref="MongoClientSettings"/> class.
    /// </summary>
    /// <param name="clientSettings">Configured settings</param>
    /// <param name="providedConfiguration">Provided configuration callback</param>
    static void ConfigureSettings(Action<MongoClientSettings>? providedConfiguration,
        MongoClientSettings clientSettings)
    {
        if (providedConfiguration is null)
        {
            clientSettings.ClusterConfigurator = builder => builder.AddTelemetry();
        }
        else
        {
            providedConfiguration.Invoke(clientSettings);
        }
    }
}
