// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionStore" />.
/// </summary>
public class ProjectionStore : IProjectionStore
{
    static readonly ProjectionsGetOne _getOneMethod = new();
    static readonly ProjectionsGetAllInBatches _getAllInBatchesMethod = new();
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

    /// <inheritdoc />
    public async Task<CurrentState<TProjection>> GetState<TProjection>(Key key, CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        var (projectionId, scopeId) = _projectionAssociations.GetFor<TProjection>();
        return await GetState<TProjection>(key, projectionId, scopeId, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<CurrentState<TProjection>> GetState<TProjection>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TProjection : class, new()
        => GetState<TProjection>(key, projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc />
    public Task<CurrentState<object>> GetState(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        => GetState<object>(key, projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc />
    public Task<CurrentState<object>> GetState(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        => GetState<object>(key, projectionId, scopeId, cancellation);

    /// <inheritdoc />
    public Task<CurrentState<TProjection>> GetState<TProjection>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TProjection : class, new()
        => GetState<TProjection>(
            () => Log.GettingOneProjectionState(_logger, key, projectionId, typeof(TProjection), scopeId),
            key,
            projectionId,
            scopeId,
            cancellation);

    /// <inheritdoc/>
    public async Task<TProjection> Get<TProjection>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        return await GetState<TProjection>(
            () => Log.GettingOneProjection(_logger, key, projectionId, typeof(TProjection), scopeId),
            key,
            projectionId,
            scopeId,
            cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TProjection>> GetAll<TProjection>(System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        var (projectionId, scopeId) = _projectionAssociations.GetFor<TProjection>();
        return await GetAll<TProjection>(projectionId, scopeId, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<TProjection>> GetAll<TProjection>(ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
        => GetAll<TProjection>(projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc/>
    public Task<IEnumerable<object>> GetAll(ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
        => GetAll<object>(projectionId, ScopeId.Default, cancellation);

    /// <inheritdoc/>
    public Task<IEnumerable<object>> GetAll(ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
        => GetAll<object>(projectionId, scopeId, cancellation);

    /// <inheritdoc/>
    public async Task<IEnumerable<TProjection>> GetAll<TProjection>(ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        Log.GettingAllProjections(
            _logger,
            projectionId,
            typeof(TProjection),
            scopeId);

        var result = new Dictionary<Key, CurrentState<TProjection>>();
        var batchNumber = 0;
        await foreach (var response in _caller.Call(
                           _getAllInBatchesMethod,
                           _requestCreator.CreateGetAll(new ScopedProjectionId(projectionId, scopeId), _executionContext),
                           cancellation))
        {
            response.Failure.ThrowIfFailureIsSet();
            Log.ProcessingProjectionsInBatch(_logger, ++batchNumber, response.States.Count);

            if (!_toSDK.TryConvert<TProjection>(response.States, out var states, out var error))
            {
                Log.FailedToConvertProjectionStates(_logger, error, response.States.Select(_ => _.State), typeof(TProjection));
                throw error;
            }
            foreach (var (key, value) in states.ToDictionary(_ => _.Key))
            {
                if (!result.TryAdd(key, value))
                {
                    throw new ReceivedDuplicateProjectionKeys(projectionId, key);
                }
            }
        }
        return result.Values.Select(_ => _.State);
    }
    
    async Task<CurrentState<TProjection>> GetState<TProjection>(
        Action logOnGetting,
        Key key,
        ProjectionId projectionId,
        ScopeId scopeId,
        CancellationToken cancellation = default)
        where TProjection : class, new()
    {
        logOnGetting();

        var response = await _caller.Call(
            _getOneMethod,
            _requestCreator.CreateGetOne(key, new ScopedProjectionId(projectionId, scopeId),_executionContext),
            cancellation).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        if (_toSDK.TryConvert<TProjection>(response.State, out var state, out var error))
        {
            ThrowIfIncorrectCurrentState(key, projectionId, state);
            return state;
        }
        Log.FailedToConvertProjectionState(_logger, error, response.State.State, typeof(TProjection));
        throw error;
    }
    static void ThrowIfIncorrectCurrentState<TProjection>(Key key, ProjectionId projectionId, CurrentState<TProjection> state)
        where TProjection : class, new()
    {
        if (!state.Key.Equals(key))
        {
            throw new WrongKeyOnProjectionCurrentState(projectionId, key, state.Key);
        }
    }
}
