// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="IServiceProvider"/> for a tenant is unable to resolve a service.
    /// </summary>
    public class MissingServiceForTenant : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingServiceForTenant"/> class.
        /// </summary>
        /// <param name="service">The service that could not be resolved.</param>
        /// <param name="tenant">The tenant the service is resolved for.</param>
        public MissingServiceForTenant(Type service, TenantId tenant)
            : base($"Could not resolved {service} for tenant {tenant}")
        {
        }
    }
}