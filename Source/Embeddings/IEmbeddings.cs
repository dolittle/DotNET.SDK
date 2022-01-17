// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Defines a system for working on an <see cref="IEmbedding" /> scoped to a specific tenant.
/// </summary>
public interface IEmbeddings
{
    /// <summary>
    /// Build an <see cref="IEmbedding" /> for the given tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <returns>The <see cref="IEmbedding" /> for that tenant.</returns>
    IEmbedding ForTenant(TenantId tenant);
}