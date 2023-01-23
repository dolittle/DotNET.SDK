// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.DependencyInversion;
using Machine.Specifications;

namespace DependencyInversion.for_TypeExtensions.when_getting_service_descriptors_from_tenant_scoped_type.and_type_is_generic;

public class implementing_closed_generic_interface : given.the_type
{
    [PerTenant]
    class type<T> : given.generic_interface<int> {}
    Establish context = () => type = typeof(type<>);

    Because of = getting_the_service_descriptors;

    It should_get_one_descriptor = () => descriptors.Length.ShouldEqual(1);
    It should_have_the_correct_implementation_type = () => descriptors[0].ImplementationType.ShouldEqual(type);
    It should_have_the_correct_service_type = () => descriptors[0].ServiceType.ShouldEqual(typeof(given.generic_interface<int>));
}