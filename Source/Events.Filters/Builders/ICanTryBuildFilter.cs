// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Defines a builder for a filter.
/// </summary>
public interface ICanTryBuildFilter : IEquatable<ICanTryBuildFilter>
{
    /// <summary>
    /// Builds the filter.
    /// </summary>
    /// <param name="identifier">The <see cref="FilterModelId"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="filter">The outputted <see cref="ICanRegisterEventFilterProcessor"/> that can register the private filter.</param>
    /// <returns>A value indicating whether the building succeeded or not.</returns>
    bool TryBuild(FilterModelId identifier, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter);
}
