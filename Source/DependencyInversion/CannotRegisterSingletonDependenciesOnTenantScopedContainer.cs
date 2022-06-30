// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Exception that gets thrown when trying to register a singleton on a tenant scoped container.
/// </summary>
public class CannotRegisterSingletonDependenciesOnTenantScopedContainer : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotRegisterSingletonDependenciesOnTenantScopedContainer"/> class.
    /// </summary>
    public CannotRegisterSingletonDependenciesOnTenantScopedContainer()
        : base("Singleton services that are not registered as a value, is not supported in per-tenant containers")
    {
    }
}
