// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Defines a system that can build the property conversions for a projection read model <see cref="Type"/>.
/// </summary>
public interface ICanBuildPropertyConversionsFromReadModel
{
    /// <summary>
    /// Builds the <see cref="PropertyConversion"/> conversions from the <typeparamref name="TReadModel"/> projection read model.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="conversions">The <see cref="IPropertyConversions"/>.</param>
    /// <returns>The default conversions derived from the <typeparamref name="TReadModel"/>.</returns>
    bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, IPropertyConversions conversions)
        where TReadModel : class, new();
}
