// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using BenchmarkDotNet.Loggers;
//using Docker.DotNet;
//
//namespace Dolittle.Benchmarks.Harness;
//
//public class RuntimeWithMongoFactory
//{
//    readonly OpenPortPool _portPool;
//    readonly ILogger _logger;
//
//    public RuntimeWithMongoFactory(OpenPortPool portPool, ILogger logger)
//    {
//        _portPool = portPool;
//        _logger = logger;
//    }
//    public RuntimeAndMongoContainer Create(IDockerClient client, string runtimeTag, string mongoDbTag)
//    {
//        return new RuntimeAndMongoContainer(
//            new RuntimeContainer(
//                client, 
//                runtimeTag, 
//                _portPool.Find(),
//                _portPool.Find(), 
//                _logger), 
//            new MongoDbContainer(
//                client,
//                mongoDbTag,
//                _portPool.Claim(27017),
//                _logger));
//    }
//
//    public RuntimeDevelopmentContainer CreateDevelopment(IDockerClient client, string tag)
//    {
//        return new RuntimeDevelopmentContainer(client, tag, _portPool.Find(), _portPool.Find(), _portPool.Find(), _logger);
//    }
//}
