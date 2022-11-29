// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="ICreateProjectionStoreRequest"/>.
/// </summary>
public class ProjectionStoreRequestCreator : ICreateProjectionStoreRequest
{
    readonly IResolveCallContext _callContextResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStoreRequestCreator"/> class.
    /// </summary>
    /// <param name="callContextResolver">The <see cref="IResolveCallContext"/>.</param>
    public ProjectionStoreRequestCreator(IResolveCallContext callContextResolver)
    {
        _callContextResolver = callContextResolver;
    }

    /// <inheritdoc />
    public GetOneRequest CreateGetOne(Key key, ProjectionId id, ScopeId scope, ExecutionContext executionContext)
        => new()
        {
            CallContext = _callContextResolver.ResolveFrom(executionContext),
            Key = key,
            ProjectionId = id.ToProtobuf(),
            ScopeId = scope.ToProtobuf()
        };

    /// <inheritdoc />
    public GetAllRequest CreateGetAll(ProjectionId id, ScopeId scope, ExecutionContext executionContext)
        => new()
        {
            CallContext = _callContextResolver.ResolveFrom(executionContext),
            ProjectionId = id.ToProtobuf(),
            ScopeId = scope.ToProtobuf()
        };
}
