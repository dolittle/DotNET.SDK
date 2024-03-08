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
public static class MongoDbCollectionNameValidator
{
    const string FailureBuildResultBeginning = "Projection MongoDB collection name cannot";

    /// <inheritdoc />
    public static bool Validate(ProjectionModelId identifier, IClientBuildResults buildResult, string collectionName)
    {
        var succeeded = true;
        if (string.IsNullOrEmpty(collectionName))
        {
            buildResult.AddFailure(identifier, $"{FailureBuildResultBeginning} be null or empty");
            return false;
        }
        if (collectionName.StartsWith("system.", StringComparison.InvariantCulture))
        {
            buildResult.AddFailure(identifier, $"{FailureBuildResultBeginning} start with 'system'.");
            succeeded = false;
        }
        if (collectionName.Contains('$'))
        {
            buildResult.AddFailure(identifier, $"{FailureBuildResultBeginning} contain '$' dollar character");
            succeeded = false;
        }
        if (collectionName.Contains('\0'))
        {
            buildResult.AddFailure(identifier, $"{FailureBuildResultBeginning} contain '\0' null character");
            succeeded = false;
        }
        if (collectionName.Length >= 64)
        {
            buildResult.AddFailure(identifier, $"{FailureBuildResultBeginning} contain more than 63 characters");
            succeeded = false;
        }
        
        return succeeded;
    }
}
