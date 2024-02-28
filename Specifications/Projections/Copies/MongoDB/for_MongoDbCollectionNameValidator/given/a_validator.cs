// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCollectionNameValidator.given;

public class a_validator
{
    protected static ClientBuildResults build_results;
    protected static ProjectionModelId identifier;
    
    
    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        identifier = new ProjectionModelId("69891694-022c-4df4-8384-55a0804ca7b4", ScopeId.Default, "aliasy");
    };
}