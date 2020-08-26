// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Artifacts
{
    /// <summary>
    /// Defines the mapper between an <see cref="Artifact"/> and a <see cref="Type"/>.
    /// </summary>
    public interface IArtifactTypeMap
    {
        /// <summary>
        /// Get an <see cref="Artifact"/> from a given type.
        /// </summary>
        /// <typeparam name="T">CLR type of the artifact.</typeparam>
        /// <returns><see cref="Artifact"/>.</returns>
        Artifact GetArtifactFor<T>();

        /// <summary>
        /// Get an <see cref="Artifact"/> from a given type.
        /// </summary>
        /// <param name="type">CLR type of the artifact.</param>
        /// <returns><see cref="Artifact"/>.</returns>
        Artifact GetArtifactFor(Type type);

        /// <summary>
        /// Get a CLR <see cref="Type"/> for a specific <see cref="Artifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to get for.</param>
        /// <returns><see cref="Type"/>.</returns>
        Type GetTypeFor(Artifact artifact);

        /// <summary>
        /// Register an relationship between a <see cref="Artifact"/> and a <see cref="Type"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to register.</param>
        /// <param name="type"><see cref="Type"/> associated with the artifact.</param>
        void Register(Artifact artifact, Type type);
    }
}