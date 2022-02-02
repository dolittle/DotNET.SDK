// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyToMongoDBBuilder{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public class ProjectionCopyToMongoDBBuilder<TReadModel> : IProjectionCopyToMongoDBBuilder<TReadModel>
    where TReadModel : class, new()
{
    readonly IValidateMongoDBCollectionName _collectionNameValidator;
    readonly IGetDefaultConversionsFromReadModel _conversionsResolver;
    readonly Dictionary<ProjectionField, Conversion> _conversions = new();
    ProjectionMongoDBCopyCollectionName _collectionName;
    bool _withoutDefaultConversions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyToMongoDBBuilder{TReadMOdel}"/> class.
    /// </summary>
    /// <param name="collectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    /// <param name="conversionsResolver">The <see cref="IGetDefaultConversionsFromReadModel"/>.</param>
    public ProjectionCopyToMongoDBBuilder(IValidateMongoDBCollectionName collectionNameValidator, IGetDefaultConversionsFromReadModel conversionsResolver)
    {
        _collectionNameValidator = collectionNameValidator;
        _conversionsResolver = conversionsResolver;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> ToCollection(ProjectionMongoDBCopyCollectionName name)
    {
        _collectionName = name;
        return this;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion(ProjectionField field, Conversion conversion)
    {
        _conversions[field] = conversion;
        return this;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion(Expression<Func<TReadModel, object>> fieldExpression, Conversion conversion)
    {
        if (fieldExpression.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression {fieldExpression.Name} refers to a method, not a class member");
        }
        return WithConversion(member.Member.Name, conversion);
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithoutDefaultConversions()
    {
        _withoutDefaultConversions = true;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copyDefinition">The built <see cref="ProjectionCopyToMongoDB"/>.</param>
    /// <returns>True if successfully built.</returns>
    public bool TryBuild(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition)
    {
        copyDefinition = default;
        var succeeded = true;
        if (_collectionName == default)
        {
            buildResults.AddFailure("MongoDB Copy collection name is not set");
            succeeded = false;
        }
        if (_collectionName != default && !_collectionNameValidator.Validate(buildResults, _collectionName))
        {
            buildResults.AddFailure($"MongoDB Copy collection name {_collectionName} is not valid");
            succeeded = false;
        }
        if (_withoutDefaultConversions)
        {
            var defaultConversions = _conversionsResolver.GetFrom<TReadModel>();
            foreach (var (field, conversion) in defaultConversions)
            {
                _conversions.TryAdd(field, conversion);
            }
        }
        if (!succeeded)
        {
            return false;
        }
        copyDefinition = new ProjectionCopyToMongoDB(true, _collectionName, _conversions);
        return true;
    }
}
