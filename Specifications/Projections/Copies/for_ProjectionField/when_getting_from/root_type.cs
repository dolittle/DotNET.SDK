// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionField.when_getting_from;

public class root_type
{
    static IDictionary<ProjectionField, MemberInfo> fields;
    
    Because of = () => fields = ProjectionField.GetAllFrom<given.root_type>();

    It should_contain_property = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.Property));
    It should_contain_property_without_setter = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.PropertyWithoutSetter));
    It should_contain_property_with_arrow = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.PropertyWithArrow));
    It should_not_contain_static_property = () => fields.Keys.ShouldNotContain<ProjectionField>(nameof(given.root_type.StaticProperty));
    It should_contain_field = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.Field));
    It should_contain_intialized_filed = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.InitializedField));
    It should_contain_readonly_field = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.ReadOnlyField));
    It should_not_contain_static_field = () => fields.Keys.ShouldNotContain<ProjectionField>(nameof(given.root_type.StaticField));
    It should_not_contain_intialized_static_field = () => fields.Keys.ShouldNotContain<ProjectionField>(nameof(given.root_type.InitializedStaticField));
    It should_contain_private_property = () => fields.Keys.ShouldContain<ProjectionField>("PrivateProperty");
    It should_contain_private_property_with_arrow = () => fields.Keys.ShouldContain<ProjectionField>("PrivatePropertyWithArrow");
    It should_contain_private_field = () => fields.Keys.ShouldContain<ProjectionField>("PrivateField");
    It should_contain_read_only_private_field = () => fields.Keys.ShouldContain<ProjectionField>("ReadOnlyPrivateField");
    It should_contain_int_array = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.IntArray));
    It should_contain_complex_field = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.ComplexField));
    It should_contain_complex_property = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.root_type.ComplexProperty));
    It should_not_contain_method = () => fields.Keys.ShouldNotContain<ProjectionField>(nameof(given.root_type.Method));
    It should_not_contain_static_method = () => fields.Keys.ShouldNotContain<ProjectionField>(nameof(given.root_type.StaticMethod));
}