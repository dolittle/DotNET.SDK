// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Resources.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Resources.Internal;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.SDK.Resources.MongoDB.Internal;

/// <summary>
/// Represents an implementation of <see cref="ResourceCreator{TResource,TRequest,TResponse}"/> for MongoDB.
/// </summary>
public class MongoDBResourceCreator : ResourceCreator<MongoDBResource, GetRequest, GetMongoDBResponse>
{
    readonly Action<MongoClientSettings> _configureClientSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBResourceCreator"/> class.
    /// </summary>
    /// <param name="methodCaller">The method caller to make requests to the Runtime with.</param>
    /// <param name="executionContext">The base execution context for the client.</param>
    /// <param name="configureClientSettings">MongoDB client settings configuration</param>
    /// <param name="loggerFactory">The logger factory to use to create loggers.</param>
    public MongoDBResourceCreator(IPerformMethodCalls methodCaller, ExecutionContext executionContext, Action<MongoClientSettings> configureClientSettings,
        ILoggerFactory loggerFactory)
        : base("MongoDB", new ResourcesGetMongoDBMethod(), methodCaller, executionContext, loggerFactory)
    {
        _configureClientSettings = configureClientSettings;
    }

    /// <inheritdoc />
    protected override GetRequest CreateResourceRequest(CallRequestContext callContext)
        => new() { CallContext = callContext };

    /// <inheritdoc />
    protected override bool RequestFailed(GetMongoDBResponse response, out Failure failure)
    {
        failure = response.Failure;
        return failure != null;
    }

    /// <inheritdoc />
    protected override MongoDBResource CreateResourceFrom(GetMongoDBResponse response)
        => new(response,_configureClientSettings);
}
