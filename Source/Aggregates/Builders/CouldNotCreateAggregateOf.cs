// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Aggregates.Builders
{
    /// <summary>
    /// Exception that gets thrown when an instance of <see cref="AggregateOf{TAggregateRoot}"/> could not be instantiated for a Tenant and a <see cref="Type"/>.
    /// </summary>
    public class CouldNotCreateAggregateOf : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotCreateAggregateOf"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        public CouldNotCreateAggregateOf(Type type, TenantId tenant)
            : base($"Failed to create instance of AggregateOf for type {type} and tenant {tenant}")
        {
        }
    }
}