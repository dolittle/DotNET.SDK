// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Tenancy;

/// <summary>
/// Delegate for getting the <see cref="IServiceProvider"/> for a tenant.
/// </summary>
public delegate Task<IServiceProvider> GetServiceProviderForTenant(TenantId tenantId);
