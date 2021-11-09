// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Protobuf;

namespace Dolittle.SDK.Tenancy.Client
{
    /// <summary>
    /// Exception that gets thrown when getting all tenants failed.
    /// </summary>
    public class FailedToGetAllTenants : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToGetAllTenants"/> class.
        /// </summary>
        /// <param name="failure">The reason for why it failed.</param>
        public FailedToGetAllTenants(string failure)
            : base($"Failed to get all tenants because {failure}")
        {
        }
    }
}