// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Defines a builder for building a projection from method callbacks.
/// </summary>
public interface IProjectionBuilder
{
    /// <summary>
    /// Defines the projection to operate in a specific <see cref="ScopeId" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilder InScope(ScopeId scopeId);

    /// <summary>
    /// Creates a <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for the specified read model type.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
    /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    /// <exception cref="ReadModelAlreadyDefinedForProjection">Is thrown when called multiple times.</exception>
    IProjectionBuilderForReadModel<TReadModel> ForReadModel<TReadModel>()
        where TReadModel : ReadModel, new();
}
