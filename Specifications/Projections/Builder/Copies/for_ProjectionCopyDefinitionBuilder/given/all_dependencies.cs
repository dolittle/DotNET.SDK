// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Castle.DynamicProxy;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.Copies.for_ProjectionCopyDefinitionBuilder.given;

public class all_dependencies
{
    protected static IClientBuildResults build_results;
    protected static ProjectionCopies result_copies;
    protected static ProjectionModelId identifier;
    protected static bool succeeded;
    
    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        identifier = new ProjectionModelId("69891694-022c-4df4-8384-55a0804ca7b2", ScopeId.Default, "some alias");
    };
    
    protected static ProjectionCopyDefinitionBuilder<TReadModel> get_definition_builder_with<TReadModel>(MongoDB.Internal.IProjectionCopyToMongoDBBuilder<TReadModel> copy_to_mongo_DB_builder)
        where TReadModel : class, new()
        => new(copy_to_mongo_DB_builder);

    protected static Mock<MongoDB.Internal.IProjectionCopyToMongoDBBuilder<TReadModel>> get_mongo_db_builder_for<TReadModel>()
        where TReadModel : class, new()
        => new();

}