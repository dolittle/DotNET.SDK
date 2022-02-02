// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCopyDefinitionFromReadModelBuilder.given;

public class all_dependencies
{
    protected static ClientBuildResults build_results;
    
    protected static MongoDbCopyDefinitionFromReadModelBuilder CopyDefinitionFromReadModelBuilder;

    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        CopyDefinitionFromReadModelBuilder = new MongoDbCopyDefinitionFromReadModelBuilder();
    };

    protected static Mock<IProjectionCopyDefinitionBuilder<TReadModel>> get_builder_for<TReadModel>(IProjectionCopyToMongoDBBuilder<TReadModel> mongo_builder)
        where TReadModel : class, new()
    {
        var mock = new Mock<IProjectionCopyDefinitionBuilder<TReadModel>>();
        mock
            .Setup(_ => _.CopyToMongoDB(Moq.It.IsAny<Action<IProjectionCopyToMongoDBBuilder<TReadModel>>>()))
            .Callback<Action<IProjectionCopyToMongoDBBuilder<TReadModel>>>(_ => _?.Invoke(mongo_builder));
        return mock;
    }
}