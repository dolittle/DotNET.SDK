// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents a delegate for creating a tenant container from a <see cref="IServiceCollection"/>.
/// </summary>
public delegate IServiceProvider CreateTenantContainer(IServiceCollection collection);
