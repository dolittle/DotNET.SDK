// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildPropertyConversionsFromReadModel"/>.
/// </summary>
public class ConversionFromBsonClassMapFromReadModelGetter : IBuildPropertyConversionsFromBsonClassMap
{
    /// <inheritdoc />
    public bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, IPropertyConversionsBuilder conversionsBuilder) where TReadModel : class, new()
        => throw new NotImplementedException();
}
