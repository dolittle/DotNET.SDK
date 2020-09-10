// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Defines a system that knows about <see cref="Artifact"/>.
    /// </summary>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact" />.</typeparam>
    public interface IArtifacts<TArtifact>
        where TArtifact : Artifact
    {
        /// <summary>
        /// Check if there is a type associated with an <see cref="Artifact" />.
        /// </summary>
        /// <typeparam name="T">CLR type of the artifact.</typeparam>
        /// <returns><see cref="bool"/>.</returns>
        bool HasFor<T>();

        /// <summary>
        /// Check if there is a type associated with an <see cref="Artifact" />.
        /// </summary>
        /// <param name="type">CLR type of the artifact.</param>
        /// <returns><see cref="bool"/>.</returns>
        bool HasFor(Type type);

        /// <summary>
        /// Get an <see cref="Artifact"/> from a given type.
        /// </summary>
        /// <typeparam name="T">CLR type of the artifact.</typeparam>
        /// <returns><see cref="Artifact"/>.</returns>
        TArtifact GetFor<T>();

        /// <summary>
        /// Get an <see cref="Artifact"/> from a given type.
        /// </summary>
        /// <param name="type">CLR type of the artifact.</param>
        /// <returns><see cref="Artifact"/>.</returns>
        TArtifact GetFor(Type type);

        /// <summary>
        /// Check if there is an <typeparamref name="TArtifact"/> associated with a <see cref="Type" />.
        /// </summary>
        /// <param name="artifact">The <typeparamref name="TArtifact"/>.</param>
        /// <returns><see cref="bool"/>.</returns>
        bool HasTypeFor(TArtifact artifact);

        /// <summary>
        /// Get a CLR <see cref="Type"/> for a specific <see cref="Artifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to get for.</param>
        /// <returns><see cref="Type"/>.</returns>
        Type GetTypeFor(TArtifact artifact);

        /// <summary>
        /// Register an relationship between an <see cref="Artifact"/> and a <see cref="Type"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> associated with the artifact.</param>
        /// <param name="artifact"><see cref="Artifact"/> to register.</param>
        void Associate(Type type, TArtifact artifact);
    }
}
