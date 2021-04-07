// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Defines a associations for projections.
    /// </summary>
    public interface IProjectionReadModelTypeAssociations
    {
        /// <summary>
        /// Associate a projection.
        /// </summary>
        /// <param name="projection">The <see cref="ProjectionId" />.</param>
        /// <param name="projectionType">The <see cref="Type" /> of the projection.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the projection.</param>
        void Associate(ProjectionId projection, Type projectionType, ScopeId scope);

        /// <summary>
        /// Associate a projection.
        /// </summary>
        /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
        /// <param name="projection">The <see cref="ProjectionId" />.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the projection.</param>
        void Associate<TReadModel>(ProjectionId projection, ScopeId scope)
            where TReadModel : class, new();

        /// <summary>
        /// Associate a projection.
        /// </summary>
        /// <param name="projectionType">The <see cref="Type" /> of the projection.</param>
        void Associate(Type projectionType);

        /// <summary>
        /// Associate a projection.
        /// </summary>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
        void Associate<TProjection>()
            where TProjection : class, new();

        /// <summary>
        /// Try get the <see cref="ProjectionId" /> associated with <typeparamref name="TProjection"/>.
        /// </summary>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
        /// <returns>The <see cref="ProjectionAssociation" />.</returns>
        ProjectionAssociation GetFor<TProjection>()
            where TProjection : class, new();

        /// <summary>
        /// Try get the <see cref="Type" /> associated with <see cref="ProjectionId" />..
        /// </summary>
        /// <param name="projection">The <see cref="ProjectionId" />.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the projection.</param>
        /// <returns>The <see cref="Type" /> of the projection.</returns>
        Type GetType(ProjectionId projection, ScopeId scope);
    }
}
