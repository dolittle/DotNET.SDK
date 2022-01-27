// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents an implementation of <see cref="IResolveProjectionCopiesDefinition"/>.
/// </summary>
public class ProjectionCopiesDefinitionResolver : IResolveProjectionCopiesDefinition
{
    readonly IEnumerable<ICanAugmentProjectionCopy> _augmenters;

    /// <summary>
    /// Initializes a new instance of the <see cref="IResolveProjectionCopiesDefinition"/> class.
    /// </summary>
    /// <param name="augmenters">The <see cref="IEnumerable{T}"/> of <see cref="ICanAugmentProjectionCopy"/>.</param>
    public ProjectionCopiesDefinitionResolver(IEnumerable<ICanAugmentProjectionCopy> augmenters)
    {
        _augmenters = augmenters;
    }

    /// <inheritdoc />
    public bool TryResolveFor<TProjection>(IClientBuildResults buildResults, out ProjectionCopies copies)
        where TProjection : class, new()
    {
        copies = new ProjectionCopies(null);
        var succeeded = true;
        foreach (var augmenter in _augmenters.Where(_ => _.CanAugment<TProjection>()))
        {
            if (augmenter.TryAugment<TProjection>(buildResults, copies, out var augmentedCopies))
            {
                copies = augmentedCopies;
                continue;
            }
            succeeded = false;
        }
        if (succeeded)
        {
            return true;
        }
        buildResults.AddFailure($"Could not resolve projection copies definition for projection for read model type {typeof(TProjection)}");
        copies = default;
        return false;
    }
}
