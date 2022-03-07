// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.given;

public class all_dependencies
{
    protected static ConversionsFromBsonClassMapBuilder builder;
    protected static bool succeeded;
    protected static ClientBuildResults build_results;
    protected static Mock<IPropertyConversions> conversions;
    protected static Mock<IAddConversionsFromBsonClassMap> conversions_from_class_map_adder;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        conversions = new Mock<IPropertyConversions>();
        conversions_from_class_map_adder = new Mock<IAddConversionsFromBsonClassMap>();
        builder = new ConversionsFromBsonClassMapBuilder(conversions_from_class_map_adder.Object);
    };
}