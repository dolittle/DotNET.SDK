// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IGetDefaultConversionsFromReadModel"/>.
/// </summary>
public class DefaultConversionsFromReadModelGetter : IGetDefaultConversionsFromReadModel
{
    /// <inheritdoc />
    public IEnumerable<PropertyConversion> GetFrom<TReadModel>() where TReadModel : class, new()
        => new List<PropertyConversion>();
}
