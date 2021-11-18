// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Exception that gets thrown when getting a resource for a tenant failed.
    /// </summary>
    public class FailedToGetResource : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToGetResource"/> class.
        /// </summary>
        /// <param name="resource">The <see cref="IResource"/>.</param>
        /// <param name="reason">The reason why it failed.</param>
        public FailedToGetResource(IResource resource, string reason)
            : base($"Failed to get resource {resource.Name.Value} for tenant {resource.Tenant.Value.ToString()} because {reason}")
        {
        }
    }
}