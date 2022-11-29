// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Defines a builder for a private filter.
/// </summary>
public interface ICanBuildPrivateFilter
{
    /// <summary>
    /// Builds the private filter.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="filter">The outputted <see cref="ICanRegisterEventFilterProcessor"/> that can register the private filter.</param>
    /// <returns>A value indicating whether the building succeeded or not.</returns>
    bool TryBuild(FilterModelId filterId, ScopeId scopeId, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter);
}
