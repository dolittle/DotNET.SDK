// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the result of a projection's On-method.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public class ProjectionResult<TReadModel> where TReadModel : class, new()
{
    /// <summary>
    /// Gets a <see cref="ProjectionResult{TReadModel}" /> that signifies that the read model should be deleted.
    /// </summary>
    public static ProjectionResult<TReadModel> Delete { get; } = new() { Type = ProjectionResultType.Delete };

    /// <summary>
    /// Gets a <see cref="ProjectionResult{TReadModel}" /> that signifies that the read model should be kept as is, IE no-op.
    /// </summary>
    public static ProjectionResult<TReadModel> Keep { get; } = new() { Type = ProjectionResultType.Keep };

    /// <summary>
    /// Gets a <see cref="ProjectionResult{TReadModel}" /> that signifies that the read model should be kept as is, IE no-op.
    /// </summary>
    public static ProjectionResult<TReadModel> Replace(TReadModel readModel) => readModel;
    
    /// <summary>
    /// Gets the updated read model.
    /// </summary>
    public TReadModel? ReadModel { get; private init; }

    /// <summary>
    /// Gets the <see cref="ProjectionResultType" />.
    /// </summary>
    public ProjectionResultType Type { get; private init; }

    /// <summary>
    /// Implicitly converts from a <typeparamref name="TReadModel"/> to a <see cref="ProjectionResult{TReadModel}" />.
    /// </summary>
    /// <param name="readModel">The updated read model instance.</param>
    public static implicit operator ProjectionResult<TReadModel>(TReadModel? readModel)
    {
        if (readModel is null)
        {
            return Delete;
        }

        return new ProjectionResult<TReadModel> { ReadModel = readModel, Type = ProjectionResultType.Replace };
    }
}
