// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Represents an implementation of <see cref="IEmbeddingStore" />.
/// </summary>
public class EmbeddingStore : IEmbeddingStore
{
    static readonly EmbeddingsGetOne _getOneMethod = new();
    static readonly EmbeddingsGetAll _getAllMethod = new();
    static readonly EmbeddingsGetKeys _getKeysMethod = new();
    readonly IPerformMethodCalls _caller;
    readonly IResolveCallContext _callContextResolver;
    readonly ExecutionContext _executionContext;
    readonly IEmbeddingReadModelTypes _embeddingAssociations;
    readonly IConvertProjectionsToSDK _toSDK;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingStore"/> class.
    /// </summary>
    /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
    /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
    /// <param name="embeddingAssociations">The <see cref="IEmbeddingReadModelTypes" />.</param>
    /// <param name="toSDK">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public EmbeddingStore(
        IPerformMethodCalls caller,
        IResolveCallContext callContextResolver,
        ExecutionContext executionContext,
        IEmbeddingReadModelTypes embeddingAssociations,
        IConvertProjectionsToSDK toSDK,
        ILogger logger)
    {
        _caller = caller;
        _callContextResolver = callContextResolver;
        _executionContext = executionContext;
        _embeddingAssociations = embeddingAssociations;
        _toSDK = toSDK;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CurrentState<TEmbedding>> Get<TEmbedding>(Key key, CancellationToken cancellation = default)
        where TEmbedding : class, new()
    {
        var embedding = _embeddingAssociations.GetFor<TEmbedding>();
        return await Get<TEmbedding>(key, embedding, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<CurrentState<object>> Get(Key key, EmbeddingId embeddingId, CancellationToken cancellation = default)
        => Get<object>(key, embeddingId, cancellation);

    /// <inheritdoc/>
    public async Task<CurrentState<TEmbedding>> Get<TEmbedding>(Key key, EmbeddingId embeddingId, CancellationToken cancellation = default)
        where TEmbedding : class, new()
    {
        _logger.Get(key, typeof(TEmbedding), embeddingId);

        var request = new GetOneRequest
        {
            CallContext = _callContextResolver.ResolveFrom(_executionContext),
            Key = key,
            EmbeddingId = embeddingId.ToProtobuf()
        };

        var response = await _caller.Call(_getOneMethod, request, cancellation).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        if (_toSDK.TryConvert<TEmbedding>(response.State, out var state, out var error))
        {
            return state;
        }
        
        _logger.FailedToConvertState(response.State.State, typeof(TEmbedding), error);
        throw error;

    }

    /// <inheritdoc/>
    public Task<IDictionary<Key, CurrentState<TEmbedding>>> GetAll<TEmbedding>(CancellationToken cancellation = default)
        where TEmbedding : class, new()
    {
        var embedding = _embeddingAssociations.GetFor<TEmbedding>();
        return GetAll<TEmbedding>(embedding, cancellation);
    }

    /// <inheritdoc/>
    public Task<IDictionary<Key, CurrentState<object>>> GetAll(EmbeddingId embeddingId, CancellationToken cancellation = default)
        => GetAll<object>(embeddingId, cancellation);

    /// <inheritdoc/>
    public async Task<IDictionary<Key, CurrentState<TEmbedding>>> GetAll<TEmbedding>(EmbeddingId embeddingId, CancellationToken cancellation = default)
        where TEmbedding : class, new()
    {
        _logger.GetAll(typeof(TEmbedding), embeddingId);

        var request = new GetAllRequest
        {
            CallContext = _callContextResolver.ResolveFrom(_executionContext),
            EmbeddingId = embeddingId.ToProtobuf()
        };

        var response = await _caller.Call(_getAllMethod, request, cancellation).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        if (_toSDK.TryConvert<TEmbedding>(response.States, out var states, out var error))
        {
            return states.ToDictionary(_ => _.Key);
        }
        
        _logger.FailedToConvertStates(response.States.Select(_ => _.State), typeof(TEmbedding), error);
        throw error;

    }

    /// <inheritdoc/>
    public Task<IEnumerable<Key>> GetKeys<TEmbedding>(CancellationToken cancellation = default)
        where TEmbedding : class, new()
    {
        var embedding = _embeddingAssociations.GetFor<TEmbedding>();
        return GetKeys<TEmbedding>(embedding, cancellation);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Key>> GetKeys(EmbeddingId embeddingId, CancellationToken cancellation = default)
        => GetKeys<object>(embeddingId, cancellation);

    /// <inheritdoc/>
    public async Task<IEnumerable<Key>> GetKeys<TEmbedding>(EmbeddingId embeddingId, CancellationToken cancellation = default)
        where TEmbedding : class, new()
    {
        _logger.GetKeys(typeof(TEmbedding), embeddingId);

        var request = new GetKeysRequest
        {
            CallContext = _callContextResolver.ResolveFrom(_executionContext),
            EmbeddingId = embeddingId.ToProtobuf()
        };

        var response = await _caller.Call(_getKeysMethod, request, cancellation).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        return response.Keys.Select(_ => (Key)_);
    }
}
