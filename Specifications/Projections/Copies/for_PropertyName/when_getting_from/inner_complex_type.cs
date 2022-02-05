using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_PropertyName.when_getting_from;

public class inner_complex_type
{
    static IDictionary<PropertyName, MemberInfo> fields;
    
    Because of = () => fields = PropertyName.GetAllFrom<given.inner_complex_type>();
    
    It should_contain_field = () => fields.Keys.ShouldContain<PropertyName>(nameof(given.inner_complex_type.NestedField));
    It should_contain_property = () => fields.Keys.ShouldContain<PropertyName>(nameof(given.inner_complex_type.NestedProperty));
}