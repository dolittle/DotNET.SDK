// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Projections.Builder.for_ProjectionCreator.given;

public class all_dependencies
{
    protected static Mock<IProjectionCopiesFromReadModelBuilders> projection_copies_resolver;
    protected static ProjectionId identifier;
    protected static ScopeId scope;
    protected static IDictionary<EventType, IProjectionMethod<given.projection_type>> on_methods;
    protected static IClientBuildResults build_results;
    protected static IProjection projection_result;
    protected static bool succeeded;
    protected static ProjectionCreator creator;
    
    Establish context = () =>
    {
        projection_copies_resolver = new Mock<IProjectionCopiesFromReadModelBuilders>();
        identifier = "C9BB598E-8FF5-4916-922B-48B8C582D725";
        scope = "CCF8DF3E-D654-4A11-8BBD-840DFEC65CED";
        on_methods = new Dictionary<EventType, IProjectionMethod<projection_type>>();
        build_results = new ClientBuildResults();
        
        creator = new ProjectionCreator(projection_copies_resolver.Object);
    };
}