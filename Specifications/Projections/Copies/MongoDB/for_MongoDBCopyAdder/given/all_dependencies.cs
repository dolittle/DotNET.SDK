// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.given;

public class all_dependencies
{
    protected static ClientBuildResults build_results;
    protected static Mock<IGetDefaultConversionsFromReadModel> conversions_resolver;
    protected static Mock<IValidateMongoDBCollectionName> collection_name_validator;
    protected static MongoDbCopyDefinitionFromReadModelBuilder CopyDefinitionFromReadModelBuilder;
    

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        conversions_resolver = new Mock<IGetDefaultConversionsFromReadModel>();
        collection_name_validator = new Mock<IValidateMongoDBCollectionName>();
        CopyDefinitionFromReadModelBuilder = new MongoDbCopyDefinitionFromReadModelBuilder(conversions_resolver.Object, collection_name_validator.Object);
    };
}