// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;
using Dolittle.Tenancy;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents an identifier for a consent given to share events between two <see cref="TenantId">tenants</see> in two <see cref="ApplicationModel.Microservice">microservices</see>.
    /// </summary>
    public class ConsentId : ConceptAs<Guid>
    {
        /// <summary>
        /// Convert a <see cref="Guid"/> to a <see cref="ConsentId"/>.
        /// </summary>
        /// <param name="consentId">The identifier of the consent.</param>
        public static implicit operator ConsentId(Guid consentId) => new ConsentId { Value = consentId };
    }
}