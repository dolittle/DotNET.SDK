// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildCopyDefinitionFromReadModel"/> that can build the <see cref="ProjectionCopyToMongoDB"/> definition.
/// </summary>
public class MongoDbCopyDefinitionFromReadModelBuilder : ICanBuildCopyDefinitionFromReadModel
{
    /// <inheritdoc />
    public bool CanBuildFrom<TReadModel>()
        where TReadModel : class, new()
        => typeof(TReadModel).TryGetDecorator<CopyProjectionToMongoDBAttribute>(out _);

    /// <inheritdoc />
    public bool BuildFrom<TReadModel>(IClientBuildResults buildResults, IProjectionCopyDefinitionBuilder<TReadModel> builder)
        where TReadModel : class, new()
    {
        var succeeded = true;
        if (!ProjectionMongoDBCopyCollectionName.TryGetFrom<TReadModel>(out var collectionName))
        {
            buildResults.AddFailure($"Could not get projection MongoDB Copy collection name from projection read model type {nameof(TReadModel)}");
            succeeded = false;
        }

        if (!succeeded)
        {
            return false;
        }
        
        builder.CopyToMongoDB(_ =>
        {
            _.ToCollection(collectionName);
            foreach (var (field, conversion) in GetExplicitConversions(ProjectionField.GetAllFrom<TReadModel>()))
            {
                _.WithConversion(field, conversion);
            }
        });
        return true;
    }

    static Dictionary<ProjectionField, Conversion> GetExplicitConversions(IDictionary<ProjectionField, MemberInfo> fields)
    => fields
        .Where(_ => Attribute.IsDefined(_.Value, typeof(MongoDBConvertToAttribute)))
        .ToDictionary(_ => _.Key, _ => ((MongoDBConvertToAttribute) Attribute.GetCustomAttribute(_.Value, typeof(MongoDBConvertToAttribute)))!.Conversion);
}
