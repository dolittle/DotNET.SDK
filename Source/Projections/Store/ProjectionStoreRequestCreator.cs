// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
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
    /// 
    /// </summary>
    /// <param name="callContextResolver"></param>
    public ProjectionStoreRequestCreator(IResolveCallContext callContextResolver)
    {
        _callContextResolver = callContextResolver;
    }

    /// <inheritdoc />
    public GetOneRequest CreateGetOne(Key key, ScopedProjectionId scopedProjectionId, ExecutionContext executionContext)
        => new()
        {
            CallContext = _callContextResolver.ResolveFrom(executionContext),
            Key = key,
            ProjectionId = scopedProjectionId.Identifier.ToProtobuf(),
            ScopeId = scopedProjectionId.ScopeId.ToProtobuf()
        };

    /// <inheritdoc />
    public GetAllRequest CreateGetAll(ScopedProjectionId scopedProjectionId, ExecutionContext executionContext)
        => new()
        {
            CallContext = _callContextResolver.ResolveFrom(executionContext),
            ProjectionId = scopedProjectionId.Identifier.ToProtobuf(),
            ScopeId = scopedProjectionId.ScopeId.ToProtobuf()
        };
}
