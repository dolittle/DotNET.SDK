// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents an implementation of <see cref="IArtifacts{TArtifact,TId}" />.
    /// </summary>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    public abstract class Artifacts<TArtifact, TId> : IArtifacts<TArtifact, TId>
        where TArtifact : Artifact<TId>
        where TId : ArtifactId
    {
        readonly IDictionary<TArtifact, Type> _artifactToTypeMap;
        readonly IDictionary<Type, TArtifact> _typeToArtifactMap;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifacts{TArtifact,TId}"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        protected Artifacts(ILogger logger)
        {
            _logger = logger;
            _artifactToTypeMap = new Dictionary<TArtifact, Type>();
            _typeToArtifactMap = new Dictionary<Type, TArtifact>();
        }

        /// <inheritdoc />
        public void Associate(Type type, TArtifact artifact)
        {
            _logger.LogTrace("Associating {Type} to {Artifact}", type, artifact);
            ThrowIfMultipleTypesAssociatedWithArtifact(artifact, type);
            ThrowIfMultipleArtifactsAssociatedWithType(type, artifact);
            _typeToArtifactMap[type] = artifact;
            _artifactToTypeMap[artifact] = type;
        }

        /// <inheritdoc />
        public TArtifact GetFor<T>()
            where T : class
            => GetFor(typeof(T));

        /// <inheritdoc />
        public TArtifact GetFor(Type type)
        {
            if (!_typeToArtifactMap.TryGetValue(type, out var artifact)) throw CreateNoArtifactAssociatedWithType(type);
            return artifact;
        }

        /// <inheritdoc />
        public Type GetTypeFor(TArtifact artifact)
        {
            if (!_artifactToTypeMap.TryGetValue(artifact, out var type)) throw CreateNoTypeAssociatedWithArtifact(artifact);
            return type;
        }

        /// <inheritdoc />
        public bool HasFor<T>()
            where T : class
            => HasFor(typeof(T));

        /// <inheritdoc />
        public bool HasFor(Type type) => _typeToArtifactMap.ContainsKey(type);

        /// <inheritdoc />
        public bool HasTypeFor(TArtifact artifact) => _artifactToTypeMap.ContainsKey(artifact);

        /// <summary>
        /// Create <see cref="Exception"/> to throw when no <typeparamref name="TArtifact"/> is associated with the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that has no association.</param>
        /// <returns>The <see cref="Exception"/> to throw.</returns>
        protected abstract Exception CreateNoArtifactAssociatedWithType(Type type);

        /// <summary>
        /// Create <see cref="Exception"/> to throw when no <see cref="Type"/> is associated with the given <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <param name="artifact">The <typeparamref name="TArtifact"/> that has no association.</param>
        /// <returns>The <see cref="Exception"/> to throw.</returns>
        protected abstract Exception CreateNoTypeAssociatedWithArtifact(TArtifact artifact);

        /// <summary>
        /// Create <see cref="Exception"/> to throw when trying to associate multiple instances of <typeparamref name="TArtifact"/> to a single <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that was attempted to associate with a <typeparamref name="TArtifact"/>.</param>
        /// <param name="artifact">The <typeparamref name="TArtifact"/> that was attempted to associate with.</param>
        /// <param name="existing">The <typeparamref name="TArtifact"/> that the <see cref="Type"/> was already associated with.</param>
        /// <returns>The <see cref="Exception"/> to throw.</returns>
        protected abstract Exception CreateCannotAssociateMultipleArtifactsWithType(Type type, TArtifact artifact, TArtifact existing);

        /// <summary>
        /// Create <see cref="Exception"/> to throw when trying to associate multiple instances of <see cref="Type"/> to a single <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <param name="artifact">The <typeparamref name="TArtifact"/> that was attempted to associate with a <see cref="Type"/>.</param>
        /// <param name="type">The <see cref="Type"/> that was attempted to associate with.</param>
        /// <param name="existing">The <see cref="Type"/> that the <typeparamref name="TArtifact"/> was already associated with.</param>
        /// <returns>The <see cref="Exception"/> to throw.</returns>
        protected abstract Exception CreateCannotAssociateMultipleTypesWithArtifact(TArtifact artifact, Type type, Type existing);

        void ThrowIfMultipleTypesAssociatedWithArtifact(TArtifact artifact, Type type)
        {
            if (_artifactToTypeMap.TryGetValue(artifact, out var existing))
            {
                throw CreateCannotAssociateMultipleTypesWithArtifact(artifact, type, existing);
            }
        }

        void ThrowIfMultipleArtifactsAssociatedWithType(Type type, TArtifact artifact)
        {
            if (_typeToArtifactMap.TryGetValue(type, out var existing))
            {
                throw CreateCannotAssociateMultipleArtifactsWithType(type, artifact, existing);
            }
        }
    }
}
