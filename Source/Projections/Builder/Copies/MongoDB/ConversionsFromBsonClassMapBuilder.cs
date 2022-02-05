// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildPropertyConversionsFromReadModel"/>.
/// </summary>
public class ConversionsFromBsonClassMapBuilder : IBuildPropertyConversionsFromBsonClassMap
{
    /// <inheritdoc />
    public bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, IPropertyConversions conversions)
        where TReadModel : class, new()
        => true;
}
