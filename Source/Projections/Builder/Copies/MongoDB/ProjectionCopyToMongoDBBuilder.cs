// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;


/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyToMongoDBBuilder{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public class ProjectionCopyToMongoDBBuilder<TReadModel> : IProjectionCopyToMongoDBBuilder<TReadModel>, Internal.IProjectionCopyToMongoDBBuilder<TReadModel>
    where TReadModel : class, new()
{
    readonly IValidateMongoDBCollectionName _collectionNameValidator;
    readonly IPropertyConversionsBuilder _conversionsBuilder;
    readonly IBuildPropertyConversionsFromBsonClassMap _conversionsFromBSONClassMap;
    readonly IMongoDBCopyDefinitionFromReadModelBuilder _mongoDbCopyFromReadModelBuilder;
    ProjectionMongoDBCopyCollectionName _collectionName;
    bool _withoutDefaultConversions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyToMongoDBBuilder{TReadMOdel}"/> class.
    /// </summary>
    /// <param name="collectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    /// <param name="conversionsBuilder">The <see cref="IPropertyConversionsBuilder"/>.</param>
    /// <param name="conversionsFromBSONClassMap">The <see cref="IBuildPropertyConversionsFromBsonClassMap"/>.</param>
    /// <param name="mongoDBCopyFromReadModelBuilder"></param>
    public ProjectionCopyToMongoDBBuilder(
        IValidateMongoDBCollectionName collectionNameValidator,
        IPropertyConversionsBuilder conversionsBuilder,
        IBuildPropertyConversionsFromBsonClassMap conversionsFromBSONClassMap,
        IMongoDBCopyDefinitionFromReadModelBuilder mongoDBCopyFromReadModelBuilder)
    {
        ConversionsBuilder = conversionsBuilder;
        _collectionNameValidator = collectionNameValidator;
        _conversionsBuilder = conversionsBuilder;
        _conversionsFromBSONClassMap = conversionsFromBSONClassMap;
        _mongoDbCopyFromReadModelBuilder = mongoDBCopyFromReadModelBuilder;
        _collectionName = ProjectionMongoDBCopyCollectionName.GetFrom<TReadModel>();
    }

    /// <inheritdoc />
    public IPropertyConversionsBuilder ConversionsBuilder { get; }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> ToCollection(ProjectionMongoDBCopyCollectionName name)
    {
        _collectionName = name;
        return this;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion<TProperty>(Expression<Func<TReadModel, TProperty>> fieldExpression, Conversion conversion)
    {
        _conversionsBuilder.AddConversion(GetPathStringFromExpression(fieldExpression), conversion);
        return this;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithoutDefaultConversions()
    {
        _withoutDefaultConversions = true;
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition)
    {
        copyDefinition = default;
        var succeeded = true;

        if (!_collectionNameValidator.Validate(buildResults, _collectionName))
        {
            buildResults.AddFailure($"MongoDB Copy collection name {_collectionName} is not valid");
            succeeded = false;
        }
        if (!_withoutDefaultConversions && !_conversionsFromBSONClassMap.TryBuildFrom<TReadModel>(buildResults, _conversionsBuilder))
        {
            buildResults.AddFailure($"MongoDB Copy failed getting default conversions based on BSON Class Map of the read model type");
            succeeded = false;
        }
        if (!succeeded)
        {
            return false;
        }
        copyDefinition = new ProjectionCopyToMongoDB(true, _collectionName, _conversionsBuilder.Build());
        return true;
    }

    /// <inheritdoc />
    public bool TryBuildFromReadModel(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition)
    {
        copyDefinition = default;
        if (_mongoDbCopyFromReadModelBuilder.CanBuild<TReadModel>())
        {
            return _mongoDbCopyFromReadModelBuilder.TryBuild(buildResults, this) && TryBuild(buildResults, out copyDefinition);
        }
        copyDefinition = ProjectionCopyToMongoDB.Default;
        return true;
    }

    static ProjectionPropertyPathString GetPathStringFromExpression<TProperty>(Expression<Func<TReadModel, TProperty>> fieldExpression)
    {
        var propertyNames = new List<ProjectionPropertyName>();
        if (fieldExpression.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression {fieldExpression.Name} refers to a method, not a class member");
        }
        propertyNames.Add(member.Member.Name);

        while (member?.Expression is MemberExpression subExpression)
        {
            member = subExpression;
            propertyNames.Add(member.Member.Name);
        }
        
        return string.Join('.', propertyNames);
    }
}
