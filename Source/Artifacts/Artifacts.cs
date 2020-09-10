// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents an implementation of <see cref="IArtifacts{TArtifact, TArtifactId}" />.
    /// </summary>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TArtifactId">The <see cref="Type" /> of <see cref="ArtifactId" />.</typeparam>
    public abstract class Artifacts<TArtifact, TArtifactId> : IArtifacts<TArtifact, TArtifactId>
        where TArtifact : Artifact<TArtifactId>
        where TArtifactId : ArtifactId
    {
        readonly IDictionary<TArtifact, Type> _artifactToTypeMap;
        readonly IDictionary<Type, TArtifact> _typeToArtifactMap;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifacts{TArtifact, TArtifactId}"/> class.
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
            => AddAssociation(type, artifact);

        /// <inheritdoc />
        public TArtifact GetFor<T>()
            where T : class
            => GetFor(typeof(T));

        /// <inheritdoc />
        public TArtifact GetFor(Type type)
        {
            if (!_typeToArtifactMap.TryGetValue(type, out var artifact)) throw new UnknownArtifact(type);
            return artifact;
        }

        /// <inheritdoc />
        public Type GetTypeFor(TArtifact artifact)
        {
            if (!_artifactToTypeMap.TryGetValue(artifact, out var type)) throw new UnknownType<TArtifactId>(artifact);
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

        void AddAssociation(Type type, TArtifact artifact)
        {
            _logger.LogTrace("Associating {Type} to {Artifact}", type, artifact);
            ThrowIfMultipleTypesAssociatedWithArtifact(artifact, type);
            ThrowIfMultipleArtifactsAssociatedWithType(type, artifact);
            _typeToArtifactMap[type] = artifact;
            _artifactToTypeMap[artifact] = type;
        }

        void ThrowIfMultipleTypesAssociatedWithArtifact(TArtifact artifact, Type type)
        {
            if (_artifactToTypeMap.ContainsKey(artifact)) throw new CannotHaveMultipleTypesAssociatedWithArtifact<TArtifactId>(artifact, type, _artifactToTypeMap[artifact]);
        }

        void ThrowIfMultipleArtifactsAssociatedWithType(Type type, TArtifact artifact)
        {
            if (_typeToArtifactMap.ContainsKey(type)) throw new CannotHaveMultipleArtifactsAssociatedWithType<TArtifactId>(type, artifact, _typeToArtifactMap[type]);
        }
    }
}
