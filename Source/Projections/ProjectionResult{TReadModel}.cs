// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Represents the result of a projection's On-method.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public class ProjectionResult<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Gets a <see cref="ProjectionResult{TReadModel}" /> that signifies that the read model should be deleted.
        /// </summary>
        public static ProjectionResult<TReadModel> Delete
            => new ProjectionResult<TReadModel> { Type = ProjectionResultType.Delete };

        /// <summary>
        /// Gets the updated read model.
        /// </summary>
        public TReadModel UpdatedReadModel { get; private set; }

        /// <summary>
        /// Gets the <see cref="ProjectionResultType" />.
        /// </summary>
        public ProjectionResultType Type { get; private set; }

        /// <summary>
        /// Implicitly converts from a <typeparamref name="TReadModel"/> to a <see cref="ProjectionResult{TReadModel}" />.
        /// </summary>
        /// <param name="readModel">The updated read model instance.</param>
        public static implicit operator ProjectionResult<TReadModel>(TReadModel readModel)
            => new ProjectionResult<TReadModel> { UpdatedReadModel = readModel, Type = ProjectionResultType.Replace };
    }
}
