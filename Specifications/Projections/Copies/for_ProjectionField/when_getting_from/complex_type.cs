using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionField.when_getting_from;

public class complex_type
{
    static IDictionary<ProjectionField, MemberInfo> fields;
    
    Because of = () => fields = ProjectionField.GetAllFrom<given.complex_type>();
    
    It should_contain_the_field = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.complex_type.NestedField));
    It should_contain_the_property = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.complex_type.NestedProperty));
    It should_contain_the_complex_field = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.complex_type.InnerComplexField));
    It should_contain_the_complex_property = () => fields.Keys.ShouldContain<ProjectionField>(nameof(given.complex_type.InnerComplexProperty));
}