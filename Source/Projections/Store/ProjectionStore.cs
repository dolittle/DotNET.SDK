// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionStore" />.
    /// </summary>
    public class ProjectionStore : IProjectionStore
    {
        static readonly ProjectionsGetOne _getOneMethod = new ProjectionsGetOne();
        static readonly ProjectionsGetAll _getAllMethod = new ProjectionsGetAll();
        readonly IPerformMethodCalls _caller;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly IProjectionReadModelTypeAssociations _projectionAssociations;
        readonly IConvertProjectionsToSDK _toSDK;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionStore"/> class.
        /// </summary>
        /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypeAssociations" />.</param>
        /// <param name="toSDK">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ProjectionStore(
            IPerformMethodCalls caller,
            IResolveCallContext callContextResolver,
            ExecutionContext executionContext,
            IProjectionReadModelTypeAssociations projectionAssociations,
            IConvertProjectionsToSDK toSDK,
            ILogger logger)
        {
            _caller = caller;
            _callContextResolver = callContextResolver;
            _executionContext = executionContext;
            _projectionAssociations = projectionAssociations;
            _toSDK = toSDK;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CurrentState<TProjection>> Get<TProjection>(Key key, System.Threading.CancellationToken cancellation = default)
            where TProjection : class, new()
        {
            var projection = _projectionAssociations.GetFor<TProjection>();
            return await Get<TProjection>(key, projection.Identifier, projection.ScopeId, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<CurrentState<TProjection>> Get<TProjection>(Key key, ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
            where TProjection : class, new()
            => Get<TProjection>(key, projectionId, ScopeId.Default, cancellation);

        /// <inheritdoc/>
        public Task<CurrentState<object>> Get(Key key, ProjectionId projectionId, System.Threading.CancellationToken cancellation = default)
            => Get<object>(key, projectionId, ScopeId.Default, cancellation);

        /// <inheritdoc/>
        public Task<CurrentState<object>> Get(Key key, ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
            => Get<object>(key, projectionId, scopeId, cancellation);

        /// <inheritdoc/>
        public async Task<CurrentState<TProjection>> Get<TProjection>(Key key, ProjectionId projectionId, ScopeId scopeId, System.Threading.CancellationToken cancellation = default)
            where TProjection : class, new()
        {
            _logger.LogDebug(
                "Getting current projection state with key {Key} for projection of {ProjectionType} with id {ProjectionId} in scope {Scope}",
                key,
                typeof(TProjection),
                projectionId,
                scopeId);

            var request = new GetOneRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Key = key,
                ProjectionId = projectionId.ToProtobuf(),
                ScopeId = scopeId.ToProtobuf()
            };

            var response = await _caller.Call(_getOneMethod, request, cancellation).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (!_toSDK.TryConvert<TProjection>(response.State, out var state, out var error))
            {
                _logger.LogError(error, "The Runtime returned the projection state '{State}'. But it could not be converted to {ProjectionType}.", response.State, typeof(TProjection));
                throw error;
            }

            return state;
        }

        /// <inheritdoc/>
        public async Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(System.Threading.CancellationToken cancellation = default)
            where TProjection : class, new()
        {
            var projection = _projectionAssociations.GetFor<TProjection>();
            return await GetAll<TProjection>(projection.Identifier, projection.ScopeId, cancellation).ConfigureAwait(false);
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

            var request = new GetAllRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                ProjectionId = projectionId.ToProtobuf(),
                ScopeId = scopeId.ToProtobuf()
            };

            var response = await _caller.Call(_getAllMethod, request, cancellation).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (!_toSDK.TryConvert<TProjection>(response.States, out var states, out var error))
            {
                _logger.LogError(error, "The Runtime returned the projection states '{States}'. But it could not be converted to {ProjectionType}.", response.States, typeof(TProjection));
                throw error;
            }

            return states.ToDictionary(_ => _.Key);
        }
    }
}
