// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCollectionNameValidator.given;

public class a_validator
{
    protected static MongoDbCollectionNameValidator validator;
    protected static ClientBuildResults build_results;
    
    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        validator = new MongoDbCollectionNameValidator();
    };
}