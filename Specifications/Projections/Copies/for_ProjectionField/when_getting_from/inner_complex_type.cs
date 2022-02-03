using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionField.when_getting_from;

public class inner_complex_type
{
    static IDictionary<ProjectionField, MemberInfo> fields;
    
    Because of = () => fields = ProjectionField.GetAllFrom<given.inner_complex_type>();
    
    It should_contain_field = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.inner_complex_type.NestedField));
    It should_contain_property = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.inner_complex_type.NestedProperty));
}