// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromConvertToMongoDBAttributesBuilder.given;

public class all_dependencies
{
    protected static ConversionsFromConvertToMongoDBAttributesBuilder builder;
    protected static ClientBuildResults build_results;
    protected static Mock<IPropertyConversions> conversions;

    Establish context = () =>
    {
        builder = new ConversionsFromConvertToMongoDBAttributesBuilder();
        conversions = new Mock<IPropertyConversions>();
        build_results = new ClientBuildResults();
    };
}