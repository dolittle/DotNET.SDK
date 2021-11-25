// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Defines an system that knows about <see cref="IServiceProvider"/> for specific Dolittle Tenants.
    /// </summary>
    public interface ITenantScopedProviders
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for a specific <see cref="TenantId"/>.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        /// <returns>The <see cref="IServiceProvider"/>.</returns>
        IServiceProvider ForTenant(TenantId tenant);
    }
}