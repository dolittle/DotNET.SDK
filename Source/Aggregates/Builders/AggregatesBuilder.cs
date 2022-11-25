// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregatesBuilder"/>.
/// </summary>
public class AggregatesBuilder : IAggregatesBuilder
{
    readonly Func<TenantId, IServiceProvider> _getServiceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatesBuilder"/> class.
    /// </summary>
    /// <param name="getServiceProvider">The <see cref="Func{TResult}"/> for getting the tenant scoped <see cref="IServiceProvider"/> for a <see cref="TenantId"/>.</param>
    public AggregatesBuilder(Func<TenantId, IServiceProvider> getServiceProvider)
    {
        _getServiceProvider = getServiceProvider;
    }

    /// <inheritdoc />
    public IAggregates ForTenant(TenantId tenant)
        => new Aggregates(_getServiceProvider(tenant));
}
