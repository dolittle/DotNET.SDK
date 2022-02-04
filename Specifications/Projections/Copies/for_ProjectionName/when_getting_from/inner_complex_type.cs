using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionName.when_getting_from;

public class inner_complex_type
{
    static IDictionary<ProjectionPropertyPath, MemberInfo> fields;
    
    Because of = () => fields = ProjectionPropertyPath.GetAllFrom<given.inner_complex_type>();
    
    It should_contain_field = () => fields.Keys.ShouldContain<ProjectionPropertyPath>(nameof(given.inner_complex_type.NestedField));
    It should_contain_property = () => fields.Keys.ShouldContain<ProjectionPropertyPath>(nameof(given.inner_complex_type.NestedProperty));
}