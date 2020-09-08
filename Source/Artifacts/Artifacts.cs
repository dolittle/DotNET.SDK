// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents an implementation of <see cref="IArtifacts" />.
    /// </summary>
    public class Artifacts : IArtifacts, IDisposable
    {
        readonly ReplaySubject<ArtifactAssociation> _registered;
        readonly BehaviorSubject<IDictionary<Type, Artifact>> _associations;
        readonly ILogger<Artifacts> _logger;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifacts"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Artifacts(ILogger<Artifacts> logger)
            : this(new Dictionary<Type, Artifact>(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Artifacts"/> class.
        /// </summary>
        /// <param name="associations">The <see cref="IDictionary{TKey, TValue}"> artifact associations </see>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Artifacts(IDictionary<Type, Artifact> associations, ILogger<Artifacts> logger)
        {
            _registered = new ReplaySubject<ArtifactAssociation>();
            _associations = new BehaviorSubject<IDictionary<Type, Artifact>>(new Dictionary<Type, Artifact>());
            _logger = logger;

            _registered.Subscribe(AddAssociation);

            foreach ((var type, var artifact) in associations) Associate(type, artifact);
        }

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
            _logger.LogDebug("Adding association between {Type} and {Artifact}", association.Type, association.Artifact);
            var map = new Dictionary<Type, Artifact>(_associations.Value)
            {
                [association.Type] = association.Artifact
            };
            _associations.OnNext(map);
        }
    }
}
