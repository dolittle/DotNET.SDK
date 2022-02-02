// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.given;

public class all_dependencies
{
    protected static ProjectionCopiesFromReadModelBuilders FromReadModelBuilder;
    protected static ProjectionCopies copies_result;
    protected static bool succeeded;
    protected static IClientBuildResults build_results; 
    protected static List<ICanBuildCopyDefinitionFromReadModel> augmenters;
    protected static Mock<ICreateCopiesDefinitionBuilder> copies_definition_builder_creator;
    
    Establish context = () =>
    {
        augmenters = new List<ICanBuildCopyDefinitionFromReadModel>();
        build_results = new ClientBuildResults();
        copies_definition_builder_creator = new Mock<ICreateCopiesDefinitionBuilder>();
        
        FromReadModelBuilder = new ProjectionCopiesFromReadModelBuilders(augmenters, copies_definition_builder_creator.Object);
    };

    protected static void add_augmenter(params Mock<ICanBuildCopyDefinitionFromReadModel>[] augmenters_to_add)
    {
        augmenters.AddRange(augmenters_to_add.Select(_ => _.Object));
    }

    protected static void use_definition_builder<TReadModel>(IProjectionCopyDefinitionBuilder<TReadModel> definition_builder)
        where TReadModel : class, new()
        => copies_definition_builder_creator
            .Setup(_ => _.CreateFor<TReadModel>()).Returns(definition_builder);

    protected delegate void try_build_callback(IClientBuildResults build_results, out ProjectionCopies copies);
}