// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Exception that gets thrown when tenant scoped container creator cannot create child container from the root <see cref="IServiceProvider"/>.
/// </summary>
public class TenantScopedContainerCreatorCannotCreateFromRootServiceProvider : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantScopedContainerCreatorCannotCreateFromRootServiceProvider"/> class.
    /// </summary>
    /// <param name="creator">The <see cref="ICanCreateTenantScopedContainer"/> that cannot create the tenant scoped container.</param>
    /// <param name="rootProvider">The root <see cref="IServiceProvider"/>.</param>
    public TenantScopedContainerCreatorCannotCreateFromRootServiceProvider(ICanCreateTenantScopedContainer creator, IServiceProvider rootProvider)
        : base($"{creator.GetType()} cannot create tenant scoped container from root provider {rootProvider.GetType()}")
    {
    }
}
