// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Services;
using Dolittle.Services.Clients;
using static Dolittle.Runtime.EventHorizon.Contracts.Subscriptions;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents something that knows about service clients.
    /// </summary>
    public class ServiceClients : IKnowAboutClients
    {
        /// <inheritdoc/>
        public IEnumerable<Client> Clients => new[]
        {
            new Client(EndpointVisibility.Private, typeof(SubscriptionsClient), Descriptor),
        };
    }
}