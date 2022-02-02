// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Defines a system that can resolve the BSON conversions for an <see cref="IProjection{TReadModel}"/> for a projection read model <see cref="Type"/>.
/// </summary>
public interface IGetDefaultConversionsFromReadModel
{
    /// <summary>
    /// Gets the conversions for fields of the <typeparamref name="TReadModel"/> projection read model <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>The default conversions derived from the <typeparamref name="TReadModel"/>.</returns>
    IDictionary<ProjectionField, Conversion> GetFrom<TReadModel>()
        where TReadModel : class, new();
}
