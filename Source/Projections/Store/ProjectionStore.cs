// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionStore" />.
/// </summary>
public class ProjectionStore : IProjectionStore
{
    static readonly ProjectionsGetOne _getOneMethod = new();
    static readonly ProjectionsGetAll _getAllMethod = new();
    readonly IPerformMethodCalls _caller;
    readonly ICreateProjectionStoreRequest _requestCreator;
    readonly ExecutionContext _executionContext;
    readonly IProjectionReadModelTypes _projectionAssociations;
    readonly IConvertProjectionsToSDK _toSDK;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStore"/> class.
    /// </summary>
    /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
    /// <param name="requestCreator">The <see cref="ICreateProjectionStoreRequest" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
    /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypes" />.</param>
    /// <param name="toSDK">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ProjectionStore(
        IPerformMethodCalls caller,
        ICreateProjectionStoreRequest requestCreator,
        ExecutionContext executionContext,
        IProjectionReadModelTypes projectionAssociations,
        IConvertProjectionsToSDK toSDK,
        ILogger logger)
    {
        _caller = caller;
        _requestCreator = requestCreator;
        _executionContext = executionContext;
        _projectionAssociations = projectionAssociations;
        _toSDK = toSDK;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TProjection> Get<TProjection>(Key key, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        var (projectionId, scopeId) = _projectionAssociations.GetFor<TProjection>();
        return await Get<TProjection>(key, projectionId, scopeId, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TProjection> Get<TProjection>(Key key, ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
        => Get<TProjection>(key, projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc/>
    public Task<object> Get(Key key, ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
        => Get<object>(key, projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc/>
    public Task<object> Get(Key key, ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
        => Get<object>(key, projectionId, scopeId, cancellation);

    /// <inheritdoc/>
    public async Task<TProjection> Get<TProjection>(Key key, ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        _logger.LogDebug(
            "Getting current projection state with key {Key} for projection of {ProjectionType} with id {ProjectionId} in scope {Scope}",
            key,
            typeof(TProjection),
            projectionId,
            scopeId);

        var response = await _caller.Call(
            _getOneMethod,
            _requestCreator.CreateGetOne(key, new ScopedProjectionId(projectionId, scopeId),_executionContext),
            cancellation).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        if (_toSDK.TryConvert<TProjection>(response.State, out var state, out var error))
        {
            // Verify that the state is expected
            return state;
        }
        _logger.LogError(error, "The Runtime returned the projection state '{State}'. But it could not be converted to {ProjectionType}.", response.State, typeof(TProjection));
        throw error;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        var (projectionId, scopeId) = _projectionAssociations.GetFor<TProjection>();
        return await GetAll<TProjection>(projectionId, scopeId, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
        => GetAll<TProjection>(projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc/>
    public Task<IDictionary<Key, CurrentState<object>>> GetAll(ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
        => GetAll<object>(projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc/>
    public Task<IDictionary<Key, CurrentState<object>>> GetAll(ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
        => GetAll<object>(projectionId, scopeId, cancellation);

    /// <inheritdoc/>
    public async Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        _logger.LogDebug(
            "Getting all current projection states for projection of {ProjectionType} with id {ProjectionId} in scope {Scope}",
            typeof(TProjection),
            projectionId,
            scopeId);

        var response = await _caller.Call(
            _getAllMethod,
            _requestCreator.CreateGetAll(new ScopedProjectionId(projectionId, scopeId),_executionContext),
            cancellation).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        if (_toSDK.TryConvert<TProjection>(response.States, out var states, out var error))
        {
            return states.ToDictionary(_ => _.Key);
        }
        _logger.LogError(error, "The Runtime returned the projection states '{States}'. But it could not be converted to {ProjectionType}.", response.States, typeof(TProjection));
        throw error;
    }
}
