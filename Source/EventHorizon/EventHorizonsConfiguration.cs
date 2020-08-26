// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;
using Dolittle.Tenancy;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents the configuration for event horizons.
    /// </summary>
    [Name("event-horizons")]
    public class EventHorizonsConfiguration : ReadOnlyDictionary<TenantId, IReadOnlyList<Subscription>>, IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="TenantId"/> with <see cref="IReadOnlyList{T}" /> of ><see cref="EventHorizon"/>.</param>
        public EventHorizonsConfiguration(IDictionary<TenantId, IReadOnlyList<Subscription>> configuration)
            : base(configuration)
        {
        }
    }
}