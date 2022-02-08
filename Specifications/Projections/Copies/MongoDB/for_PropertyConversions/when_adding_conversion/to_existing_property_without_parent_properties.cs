using System.Linq;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_PropertyConversions.when_adding_conversion;

public class to_existing_property_without_parent_properties : given.all_dependencies
{
    static PropertyName property;
    static Conversion conversion;
    
    Establish context = () =>
    {
        property = "Property";
        conversion = Conversion.GuidAsCSharpLegacyBinary;
        conversions.AddConversion(new PropertyPath(property), Conversion.DateAsInt64);
        conversions.AddConversion(new PropertyPath(property), conversion);
    };
    Because of = () => result = conversions.GetAll();

    It should_only_have_one_conversion = () => result.Count().ShouldEqual(1);
    It should_have_the_expected_property_name = () => result.First().PropertyName.ShouldEqual(property);
    It should_have_the_expected_conversion = () => result.First().ConvertTo.ShouldEqual(conversion);
    It should_not_have_rename = () => result.First().RenameTo.ShouldBeNull();
    It should_not_have_any_children = () => result.First().Children.ShouldBeEmpty();
}