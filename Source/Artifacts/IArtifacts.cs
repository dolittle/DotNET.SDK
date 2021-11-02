// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Defines a system that knows about <see cref="Artifact{TId}"/>.
    /// </summary>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    public interface IArtifacts<TArtifact, TId>
        where TArtifact : Artifact<TId>
        where TId : ArtifactId
    {
        /// <summary>
        /// Gets all artifacts.
        /// </summary>
        IEnumerable<TArtifact> All { get; }

        /// <summary>
        /// Check if there is a type associated with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <typeparam name="T">CLR type of the artifact.</typeparam>
        /// <returns><see cref="bool"/>.</returns>
        bool HasFor<T>()
            where T : class;

        /// <summary>
        /// Check if there is a type associated with an <see cref="Artifact{TId}" />.
        /// </summary>
        /// <param name="type">CLR type of the artifact.</param>
        /// <returns><see cref="bool"/>.</returns>
        bool HasFor(Type type);

        /// <summary>
        /// Get an <see cref="Artifact{TId}"/> from a given type.
        /// </summary>
        /// <typeparam name="T">CLR type of the artifact.</typeparam>
        /// <returns><see cref="Artifact{TId}"/>.</returns>
        TArtifact GetFor<T>()
            where T : class;

        /// <summary>
        /// Get an <see cref="Artifact{TId}"/> from a given type.
        /// </summary>
        /// <param name="type">CLR type of the artifact.</param>
        /// <returns><see cref="Artifact{TId}"/>.</returns>
        TArtifact GetFor(Type type);

        /// <summary>
        /// Check if there is an <typeparamref name="TArtifact"/> associated with a <see cref="Type" />.
        /// </summary>
        /// <param name="artifact">The <typeparamref name="TArtifact"/>.</param>
        /// <returns><see cref="bool"/>.</returns>
        bool HasTypeFor(TArtifact artifact);

        /// <summary>
        /// Get a CLR <see cref="Type"/> for a specific <see cref="Artifact{TId}"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact{TId}"/> to get for.</param>
        /// <returns><see cref="Type"/>.</returns>
        Type GetTypeFor(TArtifact artifact);

        /// <summary>
        /// Register an relationship between an <see cref="Artifact{TId}"/> and a <see cref="Type"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> associated with the artifact.</param>
        /// <param name="artifact"><see cref="Artifact{TId}"/> to register.</param>
        void Associate(Type type, TArtifact artifact);
    }
}
