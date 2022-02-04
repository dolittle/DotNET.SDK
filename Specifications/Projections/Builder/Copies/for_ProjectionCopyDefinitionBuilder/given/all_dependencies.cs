// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.for_ProjectionCopyDefinitionBuilder.given;

public class all_dependencies
{
    protected static Mock<IValidateMongoDBCollectionName> collection_name_validator;
    protected static Mock<ICanBuildPropertyConversionsFromReadModel> default_conversions_getter;
    protected static IClientBuildResults build_results;
    protected static ProjectionCopies result_copies;
    protected static bool succeeded;

    Establish context = () =>
    {
        collection_name_validator = new Mock<IValidateMongoDBCollectionName>();
        default_conversions_getter = new Mock<ICanBuildPropertyConversionsFromReadModel>();
        build_results = new ClientBuildResults();
    };
    
    protected static ProjectionCopyDefinitionBuilder<TReadModel> get_definition_builder_for<TReadModel>()
        where TReadModel : class, new()
        => new(collection_name_validator.Object, default_conversions_getter.Object);

}