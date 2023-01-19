// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.DependencyInversion;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInversion.for_TypeExtensions.given;

public class the_type
{
    protected static Type type;
    protected static ServiceDescriptor[] descriptors;

    Establish context = () =>
    {
        descriptors = null;
    };

    protected static void getting_the_service_descriptors() => descriptors = type.GetTenantScopedServiceDescriptors().ToArray();
}