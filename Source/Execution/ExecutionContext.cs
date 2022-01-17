// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Execution;

/// <summary>
/// Represents a <see cref="ExecutionContext"/>.
/// </summary>
/// <param name="Microservice"><see cref="Microservice"/> that is currently executing.</param>
/// <param name="Tenant"><see cref="TenantId"/> that is currently part of the <see cref="ExecutionContext"/>.</param>
/// <param name="Version"><see cref="Version" /> of the <see cref="Microservice" />.</param>
/// <param name="Environment"><see cref="Environment"/> for this <see cref="ExecutionContext"/>.</param>
/// <param name="CorrelationId"><see cref="CorrelationId"/> for this <see cref="ExecutionContext"/>.</param>
/// <param name="Claims"><see cref="Claims"/> to populate with.</param>
/// <param name="Culture"><see cref="CultureInfo"/> for the <see cref="ExecutionContext"/>.</param>
public record ExecutionContext(
    MicroserviceId Microservice,
    TenantId Tenant,
    Version Version,
    Environment Environment,
    CorrelationId CorrelationId,
    Claims Claims,
    CultureInfo Culture);
