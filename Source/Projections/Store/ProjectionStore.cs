// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionStore" />.
/// </summary>
public class ProjectionStore : IProjectionStore
{
    readonly IServiceProvider _providers;
    readonly ExecutionContext _executionContext;
    readonly IProjectionReadModelTypes _projectionAssociations;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStore"/> class.
    /// </summary>
    /// <param name="providers"></param>
    /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
    /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypes" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ProjectionStore(
        IServiceProvider providers,
        ExecutionContext executionContext,
        IProjectionReadModelTypes projectionAssociations,
        ILogger logger)
    {
        _providers = providers;
        _executionContext = executionContext;
        _projectionAssociations = projectionAssociations;
        _logger = logger;
    }

    /// <inheritdoc />
    public IProjectionOf<TProjection> Of<TProjection>()
        where TProjection : ProjectionBase, new()
        => new ProjectionOf<TProjection>(_providers.GetRequiredService<IMongoCollection<TProjection>>(), _projectionAssociations.GetFor<TProjection>());

    /// <inheritdoc />
    public IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId)
        where TReadModel : ProjectionBase, new()
        => new ProjectionOf<TReadModel>(_providers.GetRequiredService<IMongoCollection<TReadModel>>(), projectionId, ScopeId.Default);

    /// <inheritdoc />
    public IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId, ScopeId scopeId)
        where TReadModel : ProjectionBase, new()
        => new ProjectionOf<TReadModel>(_providers.GetRequiredService<IMongoCollection<TReadModel>>(), projectionId, scopeId);

    /// <inheritdoc/>
    public Task<TProjection?> Get<TProjection>(Key key, CancellationToken cancellation = default)
        where TProjection : ProjectionBase, new()
    {
        return Of<TProjection>().Get(key, cancellation);
    }

    /// <inheritdoc/>
    public IQueryable<TProjection> AsQueryable<TProjection>() where TProjection : ProjectionBase, new()
        => Of<TProjection>().AsQueryable();

    /// <inheritdoc/>
    public Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TReadModel : ProjectionBase, new()
        => Of<TReadModel>(projectionId).Get(key, cancellation);

    //
    // /// <inheritdoc/>
    // public Task<object> Get(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
    //     => Get<object>(key, projectionId, ScopeId.Default, cancellation);
    //
    // /// <inheritdoc/>
    // public Task<object> Get(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     => Get<object>(key, projectionId, scopeId, cancellation);
    //
    // /// <inheritdoc />
    // public Task<CurrentState<TProjection>> GetState<TProjection>(Key key, CancellationToken cancellation = default)
    //     where TProjection : class, new()
    // {
    //     var identifier = _projectionAssociations.GetFor<TProjection>();
    //     return GetState<TProjection>(key, identifier.Id, identifier.Scope, cancellation);
    // }
    //
    // /// <inheritdoc />
    // public Task<CurrentState<TReadModel>> GetState<TReadModel>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
    //     where TReadModel : class, new()
    //     => GetState<TReadModel>(key, projectionId, ScopeId.Default, cancellation);
    //
    // /// <inheritdoc />
    // public Task<CurrentState<object>> GetState(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
    //     => GetState<object>(key, projectionId, ScopeId.Default, cancellation);
    //
    // /// <inheritdoc />
    // public Task<CurrentState<object>> GetState(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     => GetState<object>(key, projectionId, scopeId, cancellation);
    //
    // /// <inheritdoc />
    // public Task<CurrentState<TReadModel>> GetState<TReadModel>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     where TReadModel : class, new()
    // {
    //     _logger.GettingOneProjectionState(key, projectionId, typeof(TReadModel), scopeId);
    //     return GetStateInternal<TReadModel>(
    //         key,
    //         projectionId,
    //         scopeId,
    //         cancellation);
    // }
    
    /// <inheritdoc/>
    public Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TReadModel : ProjectionBase, new()
    {
        return Of<TReadModel>(projectionId, scopeId).Get(key, cancellation);
    }

    // /// <inheritdoc/>
    // public Task<IEnumerable<TProjection>> GetAll<TProjection>(CancellationToken cancellation = default)
    //     where TProjection : class, new()
    // {
    //     var identifier = _projectionAssociations.GetFor<TProjection>();
    //     return GetAll<TProjection>(identifier.Id, identifier.Scope, cancellation);
    // }
    //
    // /// <inheritdoc/>
    // public Task<IEnumerable<TReadModel>> GetAll<TReadModel>(ProjectionId projectionId, CancellationToken cancellation = default)
    //     where TReadModel : class, new()
    //     => GetAll<TReadModel>(projectionId, ScopeId.Default, cancellation);
    //
    // /// <inheritdoc/>
    // public Task<IEnumerable<object>> GetAll(ProjectionId projectionId, CancellationToken cancellation = default)
    //     => GetAll<object>(projectionId, ScopeId.Default, cancellation);
    //
    // /// <inheritdoc/>
    // public Task<IEnumerable<object>> GetAll(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     => GetAll<object>(projectionId, scopeId, cancellation);

    /// <inheritdoc/>
    // public async Task<IEnumerable<TReadModel>> GetAll<TReadModel>(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     where TReadModel : class, new()
    // {
    //     _logger.GettingAllProjections(projectionId,
    //         typeof(TReadModel),
    //         scopeId);
    //
    //     var result = new Dictionary<Key, CurrentState<TReadModel>>();
    //     var batchNumber = 0;
    //     await foreach (var response in _caller.Call(
    //                        _getAllInBatchesMethod,
    //                        _requestCreator.CreateGetAll(projectionId, scopeId, _executionContext),
    //                        cancellation))
    //     {
    //         response.Failure.ThrowIfFailureIsSet();
    //         _logger.ProcessingProjectionsInBatch(++batchNumber, response.States.Count);
    //
    //         if (!_toSDK.TryConvert<TReadModel>(response.States, out var states, out var error))
    //         {
    //             _logger.FailedToConvertProjectionState(typeof(TReadModel), error);
    //             throw error;
    //         }
    //         foreach (var (key, value) in states.ToDictionary(_ => _.Key))
    //         {
    //             if (!result.TryAdd(key, value))
    //             {
    //                 throw new ReceivedDuplicateProjectionKeys(projectionId, key);
    //             }
    //         }
    //     }
    //     return result.Values.Select(_ => _.State);
    // }

    // async Task<CurrentState<TReadModel>> GetStateInternal<TReadModel>(
    //     Key key,
    //     ProjectionId projectionId,
    //     ScopeId scopeId,
    //     CancellationToken cancellation = default)
    //     where TReadModel : class, new()
    // {
    //     var response = await _caller.Call(
    //         _getOneMethod,
    //         _requestCreator.CreateGetOne(key, projectionId, scopeId,_executionContext),
    //         cancellation).ConfigureAwait(false);
    //     response.Failure.ThrowIfFailureIsSet();
    //
    //     if (_toSDK.TryConvert<TReadModel>(response.State, out var state, out var error))
    //     {
    //         ThrowIfIncorrectCurrentState(key, projectionId, state);
    //         return state;
    //     }
    //     _logger.FailedToConvertProjectionState(typeof(TReadModel), error);
    //     throw error;
    // }
    static void ThrowIfIncorrectCurrentState<TReadModel>(Key key, ProjectionId projectionId, CurrentState<TReadModel> state)
        where TReadModel : class, new()
    {
        if (!state.Key.Equals(key))
        {
            throw new WrongKeyOnProjectionCurrentState(projectionId, key, state.Key);
        }
    }
}
