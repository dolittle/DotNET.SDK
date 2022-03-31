// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;
using Docker.DotNet.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// Represents a MongoDB server instance running in Docker.
/// </summary>
public class MongoDB
{
    const int MongoPort = 27017;
    const string MongoImage = "dolittle/mongodb:4.2.2";
    static readonly Command<BsonDocument> _pingCommand = new BsonDocument("ping", 1);

    MongoDB(ContainerListResponse status)
    {
        Client = new MongoClient($"mongodb://localhost:{GetExposedMongoPort(status)}");
        DockerHost = status.NetworkSettings.Networks["bridge"].IPAddress;
    }
    
    /// <summary>
    /// Gets the <see cref="IMongoClient"/> to use to interact with the MongoDB server.
    /// </summary>
    public IMongoClient Client { get; }
    
    /// <summary>
    /// Gets the host of the MongoDB server from within the Docker default bridge network.
    /// </summary>
    public string DockerHost { get; }

    async Task Initialize(CancellationToken cancellationToken)
    {
        await Client.GetDatabase("admin").RunCommandAsync(_pingCommand, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Drops the databases specified by name.
    /// </summary>
    /// <param name="databases">The list of database names to drop.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to use to cancel the operation.</param>
    public async Task DropDatabases(IEnumerable<string> databases, CancellationToken cancellationToken = default)
    {
        foreach (var database in databases)
        {
            await Client.DropDatabaseAsync(database, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Starts a new or finds an existing MongoDB server container with the specified name in Docker.
    /// </summary>
    /// <param name="containerName">The container name to find or start.</param>
    /// <param name="docker">The <see cref="IDockerClient"/> to use to find or create the container.</param>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to use to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="MongoDB"/> representing the MongoDB server container.</returns>
    public static async Task<MongoDB> StartOrReuseExistingContainer(string containerName, IDockerClient docker, ILogger logger, CancellationToken cancellationToken = default)
    {
        foreach (var container in await docker.Containers.ListContainersAsync(new ContainersListParameters{ All = true }, cancellationToken).ConfigureAwait(false))
        {
            if (!container.IsNamed(containerName))
            {
                continue;
            }

            if (container.Image != MongoImage)
            {
                throw new FailedToStartMongoDB($"Found MongoDB container named {containerName}, but it is running the wrong image. Expected {MongoImage} but was {container.Image}");
            }

            if (!container.TryGetPublishedPort(MongoPort, out _))
            {
                throw new FailedToStartMongoDB($"Found MongoDB container named {containerName}, but it not exposing port {MongoPort}");
            }

            logger.WriteLineInfo("Found existing MongoDB container, ensuring it is ready...");
            var existingContainer = await docker.StartContainer(container.ID, cancellationToken).ConfigureAwait(false);
            var existingMongo = new MongoDB(existingContainer);
            await existingMongo.Initialize(cancellationToken).ConfigureAwait(false);
            logger.WriteLineInfo("MongoDB is ready");
            return existingMongo;
        }

        logger.WriteLineInfo("No MongoDB found, starting a new one...");
        var createdContainer = await docker.StartNewContainer(new CreateContainerParameters
        {
            Name = containerName,
            Image = MongoImage,
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                [$"{MongoPort}/tcp"] = new(),
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    [$"{MongoPort}/tcp"] = new List<PortBinding>{ new() },
                },
            },
        }, cancellationToken).ConfigureAwait(false);
        var newMongo = new MongoDB(createdContainer);
        await newMongo.Initialize(cancellationToken).ConfigureAwait(false);
        logger.WriteLineInfo("MongoDB is ready");
        return newMongo;
    }

    static int GetExposedMongoPort(ContainerListResponse status)
    {
        if (status.TryGetPublishedPort(MongoPort, out var publicPort))
        {
            return publicPort;
        }

        throw new FailedToStartMongoDB($"The port {MongoPort} is not published");
    }
}
