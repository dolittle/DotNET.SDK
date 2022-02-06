// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_Id_field : given.all_dependencies
{
    public class read_model
    {
        public string Id;
        public string Field;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_rename__id_to_Id = () => conversions.Verify(_ => _.AddRenaming("_id", "Id"));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
public class with_id_field : given.all_dependencies
{
    public class read_model
    {
        public string id;
        public string Field;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_rename__id_to_id = () => conversions.Verify(_ => _.AddRenaming("_id", "id"));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}