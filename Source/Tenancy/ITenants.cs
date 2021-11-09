// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy.Internal;

namespace Dolittle.SDK.Tenancy
{
    /// <summary>
    /// Defines a system that knows about Tenants in the Runtime.
    /// </summary>
    public interface ITenants
    {
        /// <summary>
        /// Gets all tenants.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="FailedToGetAllTenants">Exception that gets thrown when the Runtime failed getting all tenants.</exception>
        Task<IEnumerable<Tenant>> GetAll(CancellationToken cancellationToken);
    }
}