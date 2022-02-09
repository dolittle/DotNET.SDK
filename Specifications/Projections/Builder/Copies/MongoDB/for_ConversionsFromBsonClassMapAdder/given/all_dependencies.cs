// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.given;

public class all_dependencies
{
    protected static ConversionsFromBsonClassMapAdder adder;
    protected static ClientBuildResults build_results;
    protected static Mock<IPropertyConversions> conversions;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        conversions = new Mock<IPropertyConversions>();
        adder = new ConversionsFromBsonClassMapAdder();
    };
}