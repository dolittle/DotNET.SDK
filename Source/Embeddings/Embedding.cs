// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EmbeddingStore = Dolittle.SDK.Embeddings.Store.EmbeddingStore;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbedding" />.
    /// </summary>
    public class Embedding : EmbeddingStore, IEmbedding
    {
        static readonly EmbeddingsUpdate _updateMethod = new EmbeddingsUpdate();
        static readonly EmbeddingsDelete _deleteMethod = new EmbeddingsDelete();
        readonly IPerformMethodCalls _caller;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly IEmbeddingReadModelTypes _embeddingAssociations;
        readonly IConvertProjectionsToSDK _toSDK;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Embedding"/> class.
        /// </summary>
        /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="embeddingAssociations">The <see cref="IEmbeddingReadModelTypes" />.</param>
        /// <param name="toSDK">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public Embedding(
            IPerformMethodCalls caller,
            IResolveCallContext callContextResolver,
            ExecutionContext executionContext,
            IEmbeddingReadModelTypes embeddingAssociations,
            IConvertProjectionsToSDK toSDK,
            ILoggerFactory loggerFactory)
            : base(caller, callContextResolver, executionContext, embeddingAssociations, toSDK, loggerFactory.CreateLogger<EmbeddingStore>())
        {
            _caller = caller;
            _callContextResolver = callContextResolver;
            _executionContext = executionContext;
            _embeddingAssociations = embeddingAssociations;
            _toSDK = toSDK;
            _logger = loggerFactory.CreateLogger<Embedding>();
        }

        /// <inheritdoc/>
        public Task Delete<TEmbedding>(Key key, CancellationToken cancellation = default)
            where TEmbedding : class, new()
        {
            var embedding = _embeddingAssociations.GetFor<TEmbedding>();
            return Delete(key, embedding, cancellation);
        }

        /// <inheritdoc/>
        public async Task Delete(Key key, EmbeddingId embeddingId, CancellationToken cancellation = default)
        {
            _logger.LogDebug(
                "Deleting embedding with key {Key} and id {EmbeddingId}",
                key,
                embeddingId);

            var request = new DeleteRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Key = key,
                EmbeddingId = embeddingId.ToProtobuf()
            };

            var response = await _caller.Call(_deleteMethod, request, cancellation).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();
        }

        /// <inheritdoc/>
        public Task<CurrentState<TEmbedding>> Update<TEmbedding>(Key key, TEmbedding state, CancellationToken cancellation = default)
            where TEmbedding : class, new()
        {
            var embedding = _embeddingAssociations.GetFor<TEmbedding>();
            return Update(key, embedding, state, cancellation);
        }

        /// <inheritdoc/>
        public Task<CurrentState<object>> Update(Key key, EmbeddingId embeddingId, object state, CancellationToken cancellation = default)
            => Update<object>(key, embeddingId, state, cancellation);

        /// <inheritdoc/>
        public async Task<CurrentState<TEmbedding>> Update<TEmbedding>(Key key, EmbeddingId embeddingId, TEmbedding state, CancellationToken cancellation = default)
            where TEmbedding : class, new()
        {
            _logger.LogDebug(
                "Updating embedding state with key {Key} for embedding of {EmbeddingType} with id {EmbeddingId}",
                key,
                typeof(TEmbedding),
                embeddingId);

            var request = new UpdateRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Key = key,
                EmbeddingId = embeddingId.ToProtobuf(),
                State = JsonConvert.SerializeObject(state, Formatting.None)
            };

            var response = await _caller.Call(_updateMethod, request, cancellation).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (!_toSDK.TryConvert<TEmbedding>(response.State, out var retrieveState, out var error))
            {
                _logger.LogError(error, "The Runtime returned the embedding state '{State}'. But it could not be converted to {EmbeddingType}.", response.State, typeof(TEmbedding));
                throw error;
            }

            return retrieveState;
        }
    }
}
