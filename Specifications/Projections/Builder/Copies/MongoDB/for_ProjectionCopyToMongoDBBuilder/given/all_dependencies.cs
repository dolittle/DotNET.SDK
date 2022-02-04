// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.given;

public class all_dependencies
{
    protected static Mock<ICanBuildPropertyConversionsFromReadModel> conversions_from_read_model;
    protected static Mock<IValidateMongoDBCollectionName> collection_name_validator;
    protected static ProjectionCopyToMongoDB copy_definition_result;
    protected static IClientBuildResults build_results;
    protected static bool succeeded;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        conversions_from_read_model = new Mock<ICanBuildPropertyConversionsFromReadModel>();
        collection_name_validator = new Mock<IValidateMongoDBCollectionName>();
    };

    protected static ProjectionCopyToMongoDBBuilder<TReadModel> setup_for<TReadModel>()
        where TReadModel : class, new()
    {
        conversions_from_read_model.Setup(_ => _.TryBuildFrom<TReadModel>()).Returns(new Dictionary<ProjectionPropertyPath, Conversion>());
        collection_name_validator.Setup(_ => _.Validate(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionMongoDBCopyCollectionName>())).Returns(true);
        return new ProjectionCopyToMongoDBBuilder<TReadModel>(collection_name_validator.Object, conversions_from_read_model.Object);
    }

}