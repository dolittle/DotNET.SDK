// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Represents the result from connecting the Dolittle Client to the Dolittle Runtime.
/// </summary>
/// <param name="ExecutionContext">The initial <see cref="ExecutionContext"/> for the Dolittle Client.</param>
/// <param name="Tenants">The <see cref="IEnumerable{TResult}"/> of <see cref="Tenant"/> configured for the Dolittle Runtime.</param>
public record ConnectionResult(ExecutionContext ExecutionContext, IEnumerable<Tenant> Tenants);