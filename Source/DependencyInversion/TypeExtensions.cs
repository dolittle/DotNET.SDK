// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the <see cref="ServiceDescriptor"/> list of the tenant-scoped registrations
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <returns>The list of <see cref="ServiceDescriptor"/>.</returns>
    public static IEnumerable<ServiceDescriptor> GetTenantScopedServiceDescriptors(this Type type)
    {
        var descriptors = new List<ServiceDescriptor>();
        var attribute = type.GetCustomAttribute<PerTenantAttribute>();
        if (attribute is null)
        {
            return descriptors;
        }
        var implementors = type.GetInterfaces().Where(_ => _ != typeof(IDisposable)).ToList();
        if (attribute.RegisterAsSelf)
        {
            implementors.Add(type);
        }

        return implementors
            .Select(serviceType => serviceType is {IsGenericType: true} && serviceType.GetGenericArguments().Any(_ => _.IsGenericParameter)
                ? new ServiceDescriptor(serviceType.GetTypeInfo().GetGenericTypeDefinition(), type, attribute.Lifetime)
                : new ServiceDescriptor(serviceType, type, attribute.Lifetime));
    }
}
