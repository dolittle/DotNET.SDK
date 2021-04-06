// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionAssociations" />.
    /// </summary>
    public class ProjectionAssociations : IProjectionAssociations
    {
        readonly Dictionary<ProjectionAssociation, Type> _associationsToType = new Dictionary<ProjectionAssociation, Type>();
        readonly Dictionary<Type, ProjectionAssociation> _typeToAssociations = new Dictionary<Type, ProjectionAssociation>();

        /// <inheritdoc/>
        public void Associate(ProjectionId projection, Type projectionType, ScopeId scope)
        {
            var association = new ProjectionAssociation(projection, scope);
            ThrowIfMultipleTypesAssociatedWithProjection(association, projectionType);
            ThrowIfMultipleProjectionsAssociatedWithType(projectionType, projection);

            _associationsToType.Add(association, projectionType);
            _typeToAssociations.Add(projectionType, association);
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
        public ProjectionAssociation GetFor<TProjection>()
            where TProjection : class, new()
        {
            if (!_typeToAssociations.TryGetValue(typeof(TProjection), out var association))
            {
                throw new NoProjectionAssociatedWithType(typeof(TProjection));
            }

            return association;
        }

        /// <inheritdoc/>
        public Type GetType(ProjectionId projection, ScopeId scope)
        {
            if (!_associationsToType.TryGetValue(new ProjectionAssociation(projection, scope), out var type))
            {
                throw new NoTypeAssociatedWithProjection(projection, scope);
            }

            return type;
        }

        void ThrowIfMultipleProjectionsAssociatedWithType(Type projectionType, ProjectionId projection)
        {
            if (_typeToAssociations.TryGetValue(projectionType, out var existing))
            {
                throw new CannotAssociateMultipleProjectionsWithType(projectionType, projection, existing.Identifier);
            }
        }

        void ThrowIfMultipleTypesAssociatedWithProjection(ProjectionAssociation association, Type projectionType)
        {
            if (_associationsToType.TryGetValue(association, out var existing))
            {
                throw new CannotAssociateMultipleTypesWithProjection(association.Identifier, projectionType, existing);
            }
        }
    }
}
