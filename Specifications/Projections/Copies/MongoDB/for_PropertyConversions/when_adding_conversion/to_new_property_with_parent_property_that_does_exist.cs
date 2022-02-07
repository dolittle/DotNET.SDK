// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_PropertyConversions.when_adding_conversion;

public class to_new_property_with_parent_property_that_does_exist : given.all_dependencies
{
    static PropertyName parent;
    static PropertyName property;
    static Conversion parent_conversion;
    static Conversion conversion;
    
    Establish context = () =>
    {
        parent = "Parent";
        property = "Property";
        parent_conversion = Conversion.Date;
        conversion = Conversion.Guid;
        conversions.AddConversion(new PropertyPath(parent), parent_conversion);
        conversions.AddConversion(new PropertyPath(string.Join('.', parent, property)), conversion);
    };
    Because of = () => result = conversions.GetAll();

    It should_only_have_one_parent_conversion = () => result.Count().ShouldEqual(1);
    It should_have_the_expected_parent_property_name = () => result.First().PropertyName.ShouldEqual(parent);
    It should_have_parent_with_expected_conversion = () => result.First().ConvertTo.ShouldEqual(parent_conversion);
    It should_parent_with_no_rename = () => result.First().RenameTo.ShouldBeNull();
    It should_have_parent_with_a_child = () => result.First().Children.Count().ShouldEqual(1);
    It should_have_the_expected_property_name = () => result.First().Children.First().PropertyName.ShouldEqual(property);
    It should_have_the_expected_conversion = () => result.First().Children.First().ConvertTo.ShouldEqual(conversion);
    It should_not_have_rename = () => result.First().Children.First().RenameTo.ShouldBeNull();
}