// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents an implementation of <see cref="IArtifacts" />.
    /// </summary>
    public class Artifacts : IArtifacts, IDisposable
    {
        readonly IDictionary<Artifact, Type> _artifactToTypeMap;
        readonly Subject<ArtifactAssociation> _registered;
        readonly BehaviorSubject<IDictionary<Type, Artifact>> _associations;
        readonly ILogger<Artifacts> _logger;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifacts"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Artifacts(ILogger<Artifacts> logger)
        {
            _logger = logger;
            _artifactToTypeMap = new Dictionary<Artifact, Type>();
            _registered = new Subject<ArtifactAssociation>();
            _associations = new BehaviorSubject<IDictionary<Type, Artifact>>(new Dictionary<Type, Artifact>());

            _registered.Subscribe(AddAssociation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifacts"/> class.
        /// </summary>
        /// <param name="associations">The <see cref="IDictionary{TKey, TValue}"> artifact associations </see>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Artifacts(IDictionary<Type, Artifact> associations, ILogger<Artifacts> logger)
            : this(logger)
        {
            foreach ((var type, var artifact) in associations) Associate(type, artifact);
        }

        /// <inheritdoc/>
        public IObservable<IDictionary<Type, Artifact>> Associations => _associations;

        /// <inheritdoc />
        public void Associate(Type type, Artifact artifact)
            => _registered.OnNext(new ArtifactAssociation(type, artifact));

        /// <inheritdoc />
        public Artifact GetFor<T>() => GetFor(typeof(T));

        /// <inheritdoc />
        public Artifact GetFor(Type type)
        {
            if (!_associations.Value.TryGetValue(type, out var artifact)) throw new UnknownArtifact(type);
            return artifact;
        }

        /// <inheritdoc />
        public Type GetTypeFor(Artifact artifact)
        {
            var type = _associations.Value.FirstOrDefault(_ => _.Value == artifact).Key;
            if (type == default) throw new UnknownType(artifact);
            return type;
        }

        /// <inheritdoc />
        public bool HasFor<T>() => HasFor(typeof(T));

        /// <inheritdoc />
        public bool HasFor(Type type) => _associations.Value.ContainsKey(type);

        /// <inheritdoc />
        public bool HasTypeFor(Artifact artifact) => _associations.Value.Any(_ => _.Value == artifact);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _associations.Dispose();
                _registered.Dispose();
            }

            _disposed = true;
        }

        void AddAssociation(ArtifactAssociation association)
        {
            var previousAssociations = _associations.Value;
            if (previousAssociations.ContainsKey(association.Type))
            {
                throw new CannotHaveMultipleArtifactsAssociatedWithType(association.Type);
            }

            if (_artifactToTypeMap.ContainsKey(association.Artifact))
            {
                throw new CannotHaveMultipleTypesAssociatedWithArtifact(association.Artifact);
            }

            var map = new Dictionary<Type, Artifact>(previousAssociations)
            {
                [association.Type] = association.Artifact
            };
            _logger.LogDebug("Associating {Type} to {Artifact}", association.Type, association.Artifact);
            _artifactToTypeMap[association.Artifact] = association.Type;
            _associations.OnNext(map);
        }
    }
}
