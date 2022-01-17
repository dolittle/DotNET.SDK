// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Exception that gets thrown when <see cref="ITenantScopedProviders" /> could not instantitate projection.
/// </summary>
public class CouldNotInstantiateProjection : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotInstantiateProjection"/> class.
    /// </summary>
    /// <param name="projectionType">The <see cref="Type" /> of the projection.</param>
    public CouldNotInstantiateProjection(Type projectionType)
        : base($"{projectionType} could not be instantiated by IoC container.")
    {
    }
}