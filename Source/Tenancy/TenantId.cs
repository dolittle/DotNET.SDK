// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Tenancy
{
    /// <summary>
    /// Represents the concept of a tenant.
    /// </summary>
    public class TenantId : ConceptAs<Guid>
    {
        /// <summary>
        /// The <see cref="TenantId"/> used when it is not possible to resolve a tenant.
        /// </summary>
        public static readonly TenantId Unknown = "762a4bd5-2ee8-4d33-af06-95806fb73f4e";

        /// <summary>
        /// The <see cref="TenantId"/> used outside the scope of a tenant, typically the system.
        /// </summary>
        public static readonly TenantId System = "08831584-e016-42f6-bc5e-c4f098fed42b";

        /// <summary>
        /// A <see cref="TenantId"/> that can be used for convenience during development.
        /// </summary>
        public static readonly TenantId Development = "445f8ea8-1a6f-40d7-b2fc-796dba92dc44";

        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="TenantId"/>.
        /// </summary>
        /// <param name="tenantId"><see cref="Guid"/> representation of a tenant identifier.</param>
        public static implicit operator TenantId(Guid tenantId) => new TenantId { Value = tenantId };

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="TenantId"/>.
        /// </summary>
        /// <param name="tenantId"><see cref="string"/> representation of a tenant identifier.</param>
        public static implicit operator TenantId(string tenantId) => new TenantId { Value = Guid.Parse(tenantId) };
    }
}
