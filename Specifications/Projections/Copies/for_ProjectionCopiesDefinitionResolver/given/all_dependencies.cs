// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesDefinitionResolver.given;

public class all_dependencies
{
    protected static ProjectionCopiesFromReadModelBuilders FromReadModelBuilder;
    protected static ProjectionCopies copies_result;
    protected static bool succeeded;
    protected static IClientBuildResults build_results; 
    protected static List<ICanBuildCopyDefinitionFromReadModel> augmenters;
    
    Establish context = () =>
    {
        augmenters = new List<ICanBuildCopyDefinitionFromReadModel>();
        build_results = new ClientBuildResults();
        FromReadModelBuilder = new ProjectionCopiesFromReadModelBuilders(augmenters);
    };

    protected static void add_augmenter(params Mock<ICanBuildCopyDefinitionFromReadModel>[] augmenters_to_add)
    {
        augmenters.AddRange(augmenters_to_add.Select(_ => _.Object));
    }
}