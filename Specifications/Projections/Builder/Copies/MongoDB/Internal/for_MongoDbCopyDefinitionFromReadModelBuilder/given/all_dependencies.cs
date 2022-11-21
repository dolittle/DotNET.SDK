// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal.for_MongoDbCopyDefinitionFromReadModelBuilder.given;

public class all_dependencies
{
    protected static ClientBuildResults build_results;
    protected static MongoDBCopyDefinitionFromReadModelBuilder builder;
    protected static Mock<IBuildPropertyConversionsFromMongoDBConvertToAttributes> conversions_from_convert_to_attributes;
    protected static ProjectionModelId identifier;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        conversions_from_convert_to_attributes = new();
        builder = new MongoDBCopyDefinitionFromReadModelBuilder(conversions_from_convert_to_attributes.Object);
        identifier = new ProjectionModelId("69891694-022c-4df4-8384-55a0804ca7b0", ScopeId.Default, "some alias3");
    };
}