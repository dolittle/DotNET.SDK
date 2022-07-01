// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Exception that gets thrown when there are multiple tenant scoped container creators set up on the host.
/// </summary>
public class MultipleTenantScopedContainerCreatorsUsed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleTenantScopedContainerCreatorsUsed"/>
    /// </summary>
    public MultipleTenantScopedContainerCreatorsUsed()
        : base("There are multiple tenant scoped container providers set up. Please use only one")
    {}
}
