// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents an implementation of <see cref="IArtifactTypeMap"/>.
    /// </summary>
    public class ArtifactTypeMap : IArtifactTypeMap
    {
        readonly Dictionary<ArtifactId, Type> _typesByArtifact = new Dictionary<ArtifactId, Type>();
        readonly Dictionary<Type, Artifact> _artifactByTypes = new Dictionary<Type, Artifact>();

        /// <inheritdoc/>
        public Artifact GetArtifactFor<T>() => GetArtifactFor(typeof(T));

        /// <inheritdoc/>
        public Artifact GetArtifactFor(Type type)
        {
            return _artifactByTypes[type];
        }

        /// <inheritdoc/>
        public Type GetTypeFor(Artifact artifact)
        {
            return _typesByArtifact[artifact.Id];
        }

        /// <inheritdoc/>
        public void Register(Artifact artifact, Type type)
        {
            _typesByArtifact[artifact.Id] = type;
            _artifactByTypes[type] = artifact;
        }
    }
}