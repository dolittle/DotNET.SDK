// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IValidateMongoDBCollectionName"/>.
/// </summary>
/// <remarks>
/// Rules based on https://docs.mongodb.com/manual/reference/limits/
/// </remarks>
public class MongoDbCollectionNameValidator : IValidateMongoDBCollectionName
{
    const string FailureBuildResultBeginning = "Projection MongoDB collection name cannot";

    /// <inheritdoc />
    public bool Validate(IClientBuildResults buildResult, ProjectionMongoDBCopyCollectionName collectionName)
    {
        var succeeded = true;
        if (collectionName.Value.StartsWith("system.", StringComparison.InvariantCulture))
        {
            buildResult.AddFailure($"{FailureBuildResultBeginning} start with system.");
            succeeded = false;
        }
        if (collectionName.Value.Contains('$'))
        {
            buildResult.AddFailure($"{FailureBuildResultBeginning} contain '$' dollar character");
            succeeded = false;
        }
        if (collectionName.Value.Contains('\0'))
        {
            buildResult.AddFailure($"{FailureBuildResultBeginning} contain '\0' null character");
            succeeded = false;
        }
        if (collectionName.Value.Length >= 64)
        {
            buildResult.AddFailure($"{FailureBuildResultBeginning} contain more than 63 characters");
            succeeded = false;
        }
        if (!string.IsNullOrEmpty(collectionName.Value))
        {
            return succeeded;
        }
        
        buildResult.AddFailure($"{FailureBuildResultBeginning} be null or empty");
        return false;
    }
}
