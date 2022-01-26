// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Exception that gets thrown when an instance of <see cref="ProjectionOf{TReadModel}"/> could not be instantiated for a Tenant and a <see cref="Type"/>.
/// </summary>
public class CouldNotCreateProjectionOf : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotCreateProjectionOf"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    public CouldNotCreateProjectionOf(Type type, TenantId tenant)
        : base($"Failed to create instance of {typeof(ProjectionOf<>).Name} for type {type} and tenant {tenant}")
    {
    }
}
