// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Indicates that the class should be registered as a per-tenant dependency in a DI container.
/// Meaning that instances will not be shared across execution contexts for different tenants.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PerTenantAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerTenantAttribute"/> class.
    /// </summary>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the tenant-scoped service.</param>
    /// <param name="registerAsSelf">Whether this service should be registered in IoC container as itself or not.</param>
    public PerTenantAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient, bool registerAsSelf = false)
    {
        Lifetime = lifetime;
        RegisterAsSelf = registerAsSelf;
    }

    /// <summary>
    /// Gets the <see cref="ServiceLifetime"/>.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Gets a value indicating whether this service should be registered as itself.
    /// </summary>
    public bool RegisterAsSelf { get; }
}
