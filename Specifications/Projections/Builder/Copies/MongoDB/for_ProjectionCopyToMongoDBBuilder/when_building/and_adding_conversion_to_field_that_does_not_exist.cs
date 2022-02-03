// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building;

public class and_adding_conversion_to_field_that_does_not_exist : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static ProjectionMongoDBCopyCollectionName name_of_type;
    static ProjectionField field;
    static Conversion conversion;
    
    Establish context = () =>
    {
        builder = setup_for<given.read_model_type>();
        name_of_type = nameof(given.read_model_type);
        field = "SomeField";
        conversion = Conversion.Guid;
        builder.WithConversion(field, conversion);
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_fail = () => succeeded.ShouldBeFalse();
    It should_not_output_a_copy_definition = () => copy_definition_result.ShouldBeNull();
    It should_have_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}