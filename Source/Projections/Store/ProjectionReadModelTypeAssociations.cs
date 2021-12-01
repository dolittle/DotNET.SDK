// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionReadModelTypeAssociations" />.
    /// </summary>
    public class ProjectionReadModelTypeAssociations : IProjectionReadModelTypeAssociations
    {
        readonly Dictionary<Type, ScopedProjectionIdentifier> _typeToAssociations = new Dictionary<Type, ScopedProjectionIdentifier>();

        /// <inheritdoc/>
        public void Associate(ProjectionId projection, Type projectionType, ScopeId scope)
        {
            ThrowIfMultipleProjectionsAssociatedWithType(projectionType, projection);
            _typeToAssociations[projectionType] = new ScopedProjectionIdentifier(projection, scope);
        }

        /// <inheritdoc/>
        public void Associate<TReadModel>(ProjectionId projection, ScopeId scope)
            where TReadModel : class, new()
            => Associate(projection, typeof(TReadModel), scope);

        /// <inheritdoc/>
        public void Associate<TProjection>()
            where TProjection : class, new()
            => Associate(typeof(TProjection));

        /// <inheritdoc/>
        public void Associate(Type projectionType)
        {
            var projectionAttribute = projectionType.GetCustomAttributes(typeof(ProjectionAttribute), true)
                .FirstOrDefault() as ProjectionAttribute;

            if (projectionAttribute == default)
            {
                throw new TypeIsNotAProjection(projectionType);
            }

            Associate(projectionAttribute.Identifier, projectionType, projectionAttribute.Scope);
        }

        /// <inheritdoc/>
        public ScopedProjectionIdentifier GetFor<TProjection>()
            where TProjection : class, new()
        {
            if (!_typeToAssociations.TryGetValue(typeof(TProjection), out var association))
            {
                throw new NoProjectionAssociatedWithType(typeof(TProjection));
            }

            return association;
        }

        void ThrowIfMultipleProjectionsAssociatedWithType(Type projectionType, ProjectionId projection)
        {
            if (_typeToAssociations.TryGetValue(projectionType, out var existing))
            {
                throw new CannotAssociateMultipleProjectionsWithType(projectionType, projection, existing.Identifier);
            }
        }
    }
}
