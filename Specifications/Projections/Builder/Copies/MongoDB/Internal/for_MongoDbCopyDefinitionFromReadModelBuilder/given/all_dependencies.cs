// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal.for_MongoDbCopyDefinitionFromReadModelBuilder.given;

public class all_dependencies
{
    protected static ClientBuildResults build_results;
    protected static MongoDBCopyDefinitionFromReadModelBuilder builder;
    protected static Mock<IBuildPropertyConversionsFromMongoDBConvertToAttributes> conversions_from_convert_to_attributes;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        conversions_from_convert_to_attributes = new();
        builder = new MongoDBCopyDefinitionFromReadModelBuilder(conversions_from_convert_to_attributes.Object);
    };
}