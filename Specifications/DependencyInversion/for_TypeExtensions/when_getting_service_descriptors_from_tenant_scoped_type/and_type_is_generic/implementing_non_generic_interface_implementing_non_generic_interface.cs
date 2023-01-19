using System.Linq;
using Dolittle.SDK.DependencyInversion;
using Machine.Specifications;

namespace DependencyInversion.for_TypeExtensions.when_getting_service_descriptors_from_tenant_scoped_type.and_type_is_generic;

public class implementing_non_generic_interface_implementing_non_generic_interface : given.the_type
{
    [PerTenant]
    class type<T> : given.non_generic_interface_implementing_non_generic_interface {}

    Establish context = () => type = typeof(type<>);

    Because of = getting_the_service_descriptors;

    It should_get_two_descriptors = () => descriptors.Length.ShouldEqual(2);
    It should_have_both_be_the_correct_implementation_type = () => descriptors.ShouldEachConformTo(_ => _.ImplementationType == type);
    It should_have_one_service_type_be_non_generic_interface_implementing_non_generic_interface = () => descriptors.Any(_ => _.ServiceType == typeof(given.non_generic_interface_implementing_non_generic_interface)).ShouldBeTrue();
    It should_have_one_service_type_be_non_generic_interface = () => descriptors.Any(_ => _.ServiceType == typeof(given.non_generic_interface)).ShouldBeTrue();
}