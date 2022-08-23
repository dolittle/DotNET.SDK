// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Exception that gets thrown when a root container cannot be resolved from a service provider.
/// </summary>
public class CannotResolveContainerFromServiceProvider : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotResolveContainerFromServiceProvider"/> class.
    /// </summary>
    /// <param name="rootContainerType">The root container type.</param>
    /// <param name="rootProvider">The root <see cref="IServiceProvider"/> container.</param>
    public CannotResolveContainerFromServiceProvider(Type rootContainerType, IServiceProvider rootProvider)
        : base($"Cannot resolve root container of type {rootContainerType} from root provider of type {rootProvider.GetType()}")
    {
    }
}
