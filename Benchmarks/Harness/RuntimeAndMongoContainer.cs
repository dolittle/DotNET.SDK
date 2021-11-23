// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Benchmarks.Harness;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Cannot really use this until there is a way of binding up resources.json because of the need to setup custom mongodb port.
/// </remarks>
public class RuntimeAndMongoContainer : IRuntimeWithMongo
{
    readonly RuntimeContainer _runtimeContainer;
    readonly MongoDbContainer _mongoDbContainer;

    public RuntimeAndMongoContainer(
        RuntimeContainer runtimeContainer,
        MongoDbContainer mongoDbContainer)
    {
        _runtimeContainer = runtimeContainer;
        _mongoDbContainer = mongoDbContainer;
        Endpoints = runtimeContainer.Endpoints;
    }

    public RuntimeEndpoints Endpoints { get; }

    public Task Start()
        => Task.WhenAll(_runtimeContainer.Start(), _mongoDbContainer.Start());

    public Task Stop()
        => Task.WhenAll(_runtimeContainer.Stop(), _mongoDbContainer.Stop());

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.WhenAll(_runtimeContainer.DisposeAsync().AsTask(), _mongoDbContainer.DisposeAsync().AsTask()));
    }
}