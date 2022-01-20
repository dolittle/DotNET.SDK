// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Tenancy;

/// <summary>
/// Represents a tenant.
/// </summary>
/// <param name="Id">The <see cref="TenantId"/> of the tenant.</param>
public record Tenant(TenantId Id)
{
}
