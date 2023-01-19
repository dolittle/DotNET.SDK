// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.DependencyInversion;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInversion.for_TypeExtensions.when_getting_service_descriptors_from_tenant_scoped_type;


public class and_type_is_not_tenant_scoped : given.the_type
{
    class service : given.non_generic_interface{}

    Establish context = () => type = typeof(service);

        Because of = getting_the_service_descriptors;

    It should_not_get_any_descriptors = () => descriptors.ShouldBeEmpty();
}