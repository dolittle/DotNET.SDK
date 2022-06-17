// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Execution;

/// <summary>
/// Extension methods for <see cref="ExecutionContext"/> to build <see cref="ExecutionContext"/> with new context information.
/// </summary>
public static class ExecutionContextExtensions
{
    /// <summary>
    /// Creates a new execution context based on the current with a new <see cref="TenantId"/>.
    /// </summary>
    /// <param name="current">The current execution context to extend.</param>
    /// <param name="tenant">The <see cref="TenantId"/> to use.</param>
    /// <returns>A new <see cref="ExecutionContext"/> with the provided <see cref="TenantId"/> set.</returns>
    public static ExecutionContext ForTenant(this ExecutionContext current, TenantId tenant)
        => new(
            current.Microservice,
            tenant,
            current.Version,
            current.Environment,
            current.CorrelationId,
            current.Claims,
            current.Culture,
            current.SpanId);

    /// <summary>
    /// Creates a new execution context based on the current with a new <see cref="CorrelationId"/>.
    /// </summary>
    /// <param name="current">The current execution context to extend.</param>
    /// <param name="correlation">The <see cref="CorrelationId"/> to use.</param>
    /// <returns>A new <see cref="ExecutionContext"/> with the provided <see cref="CorrelationId"/> set.</returns>
    public static ExecutionContext ForCorrelation(this ExecutionContext current, CorrelationId correlation)
        => new(
            current.Microservice,
            current.Tenant,
            current.Version,
            current.Environment,
            correlation,
            current.Claims,
            current.Culture,
            current.SpanId);

    /// <summary>
    /// Creates a new execution context based on the current with new <see cref="Claims"/>.
    /// </summary>
    /// <param name="current">The current execution context to extend.</param>
    /// <param name="claims">The <see cref="Claims"/> to use.</param>
    /// <returns>A new <see cref="ExecutionContext"/> with the provided <see cref="Claims"/> set.</returns>
    public static ExecutionContext ForClaims(this ExecutionContext current, Claims claims)
        => new(
            current.Microservice,
            current.Tenant,
            current.Version,
            current.Environment,
            current.CorrelationId,
            claims,
            current.Culture,
            current.SpanId);
}
