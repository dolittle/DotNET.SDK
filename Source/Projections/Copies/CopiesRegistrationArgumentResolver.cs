// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies;

public class ProjectionCopiesDefinitionResolver : IResolveProjectionCopiesDefinition
{
    readonly IEnumerable<ICanAugmentProjectionCopy> _augmenters;

    public ProjectionCopiesDefinitionResolver(IEnumerable<ICanAugmentProjectionCopy> augmenters)
    {
        _augmenters = augmenters;
    }

    /// <inheritdoc />
    public bool TryResolveFor<TProjection>(IClientBuildResults buildResults, out ProjectionCopies copies)
        where TProjection : class, new()
        => throw new NotImplementedException();
}
