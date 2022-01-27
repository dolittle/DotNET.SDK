// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanAugmentProjectionCopy"/> that adds the <see cref="ProjectionCopyToMongoDB"/> definition.
/// </summary>
public class MongoDBCopyAdder : ICanAugmentProjectionCopy
{
    readonly IResolveConversions _conversionsResolver;
    readonly IValidateMongoDBCollectionName _collectionNameValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBCopyAdder"/> class.
    /// </summary>
    /// <param name="conversionsResolver">The <see cref="IResolveConversions" />.</param>
    /// <param name="collectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    public MongoDBCopyAdder(IResolveConversions conversionsResolver, IValidateMongoDBCollectionName collectionNameValidator)
    {
        _conversionsResolver = conversionsResolver;
        _collectionNameValidator = collectionNameValidator;
    }

    /// <inheritdoc />
    public bool CanAugment<TProjection>()
        where TProjection : class, new()
        => typeof(TProjection).TryGetDecorator<CopyProjectionToMongoDBAttribute>(out _);

    /// <inheritdoc />
    public bool TryAugment<TProjection>(IClientBuildResults buildResults, ProjectionCopies projectionCopies, out ProjectionCopies augmentedProjectionCopies)
        where TProjection : class, new()
    {
        var succeeded = true;
        if (!ProjectionMongoDBCopyCollectionName.TryGetFrom<TProjection>(out var collectionName))
        {
            buildResults.AddFailure($"Could not get projection MongoDB Copy collection name from projection read model type {nameof(TProjection)}");
            succeeded = false;
        }
        if (collectionName != default && !_collectionNameValidator.Validate(buildResults, collectionName))
        {
            buildResults.AddFailure($"MongoDB Copy collection name {collectionName} from projection read model type {nameof(TProjection)} is not valid");
            succeeded = false;
        }
        if (!_conversionsResolver.TryResolve<TProjection>(buildResults, out var conversions))
        {
            buildResults.AddFailure($"Could not get projection MongoDB Copy conversions from projection read model type {nameof(TProjection)}");
            succeeded = false;
        }

        if (!succeeded)
        {
            augmentedProjectionCopies = default;
            return false;
        }

        augmentedProjectionCopies = projectionCopies with
        {
            MongoDB = new ProjectionCopyToMongoDB(collectionName, conversions)
        };

        return true;
    }
}
