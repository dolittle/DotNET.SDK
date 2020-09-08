// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Defines a system for working with <see cref="Artifact"/>.
    /// </summary>
    public interface IArtifacts
    {
        /// <summary>
        /// Gets the <see cref="BehaviorSubject{T}" /> of associated artifacts.
        /// </summary>
        BehaviorSubject<IDictionary<Type, Artifact>> Associations { get; }

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
        Artifact GetFor<T>();

        /// <summary>
        /// Get an <see cref="Artifact"/> from a given type.
        /// </summary>
        /// <param name="type">CLR type of the artifact.</param>
        /// <returns><see cref="Artifact"/>.</returns>
        Artifact GetFor(Type type);

        /// <summary>
        /// Get a CLR <see cref="Type"/> for a specific <see cref="Artifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to get for.</param>
        /// <returns><see cref="bool"/>.</returns>
        bool HasTypeFor(Artifact artifact);

        /// <summary>
        /// Get a CLR <see cref="Type"/> for a specific <see cref="Artifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to get for.</param>
        /// <returns><see cref="Type"/>.</returns>
        Type GetTypeFor(Artifact artifact);

        /// <summary>
        /// Register an relationship between an <see cref="Artifact"/> and a <see cref="Type"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> associated with the artifact.</param>
        /// <param name="artifact"><see cref="Artifact"/> to register.</param>
        void Associate(Type type, Artifact artifact);
    }
}
