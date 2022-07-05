// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Exception that gets thrown when tenant containers cannot be created from the root <see cref="IServiceProvider"/> container.
/// </summary>
public class CannotCreateTenantContainersFromRootContainer<TContainer> : Exception
    where TContainer : class, IServiceProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotCreateTenantContainersFromRootContainer{TContainer}"/> class.
    /// </summary>
    /// <param name="rootProvider">The root <see cref="IServiceProvider"/> container.</param>
    public CannotCreateTenantContainersFromRootContainer(IServiceProvider rootProvider)
        : base($"Cannot create tenant containers from root container {rootProvider.GetType()}. Expected root container to be of type {typeof(TContainer)}")
    {
    }
}
