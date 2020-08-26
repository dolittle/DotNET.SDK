// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.AspNetCore.Execution
{
    /// <summary>
    /// Exception that gets thrown when there is a Tenant ID header on the HTTP request with multiple values.
    /// </summary>
    public class TenantIdHeaderHasMultipleValues : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantIdHeaderHasMultipleValues"/> class.
        /// </summary>
        /// <param name="header">The name of the HTTP header.</param>
        public TenantIdHeaderHasMultipleValues(string header)
            : base($"There are multiple values for Tenant ID header '{header}'")
        {
        }
    }
}
