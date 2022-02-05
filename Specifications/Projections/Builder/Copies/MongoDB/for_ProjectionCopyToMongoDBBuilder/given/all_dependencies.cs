// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.given;

public class all_dependencies
{
    protected static Mock<IBuildPropertyConversionsFromBsonClassMap> conversions_from_bson_class_map;
    protected static Mock<IMongoDBCopyDefinitionFromReadModelBuilder> from_read_model_builder;
    protected static Mock<IValidateMongoDBCollectionName> collection_name_validator;
    protected static Mock<IResolvePropertyPath> property_path_resolver;
    protected static ProjectionCopyToMongoDB copy_definition_result;
    protected static IClientBuildResults build_results;
    protected static bool succeeded;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        collection_name_validator = new Mock<IValidateMongoDBCollectionName>();
        conversions_from_bson_class_map = new Mock<IBuildPropertyConversionsFromBsonClassMap>();
        from_read_model_builder = new Mock<IMongoDBCopyDefinitionFromReadModelBuilder>();
        property_path_resolver = new Mock<IResolvePropertyPath>();
    };

    protected static ProjectionCopyToMongoDBBuilder<TReadModel> setup_for<TReadModel>()
        where TReadModel : class, new()
    {
        conversions_from_bson_class_map.Setup(_ => _.TryBuildFrom<TReadModel>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<PropertyConversions>())).Returns(true);
        collection_name_validator.Setup(_ => _.Validate(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionMongoDBCopyCollectionName>())).Returns(true);
        return new ProjectionCopyToMongoDBBuilder<TReadModel>(collection_name_validator.Object, conversions_from_bson_class_map.Object, from_read_model_builder.Object, property_path_resolver.Object);
    }

    protected static Expression<Func<TReadModel, TProperty>> property_path_expression_for<TReadModel, TProperty>(Expression<Func<TReadModel, TProperty>> expr)
        => expr;

    protected static void should_only_contain_conversions(params (Conversion conversion, PropertyPath path)[] conversions)
    {
        copy_definition_result.Conversions.Count().ShouldEqual(conversions.Length);
        foreach (var (conversion, path) in conversions)
        {
            copy_definition_result.Conversions.ShouldContain(_ => _.ConvertTo == conversion && _.PropertyName == path.GetParts().Last());
        }
    }

    protected static void with_explicit_conversions<TReadModel, TProperty>(ProjectionCopyToMongoDBBuilder<TReadModel> builder, params (Expression<Func<TReadModel, TProperty>>, PropertyPath, Conversion)[] conversions)
        where TReadModel : class, new()
    {
        foreach (var (expression, path, conversion) in conversions)
        {
            property_path_resolver
                .Setup(_ => _.FromExpression(expression))
                .Returns(path);
            builder.WithConversion(expression, conversion);
        }
    }
}