// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.AspNetCore.Execution
{
    /// <summary>
    /// Exception that gets thrown when there is an invalid Tenant ID in the Tenant ID header on the HTTP request.
    /// </summary>
    public class TenantIdHeaderHasInvalidTenantId : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantIdHeaderHasInvalidTenantId"/> class.
        /// </summary>
        /// <param name="header">The name of the HTTP header.</param>
        public TenantIdHeaderHasInvalidTenantId(string header)
            : base($"The Tenant ID header '{header}' contains an invalid Tenant ID")
        {
        }
    }
}
