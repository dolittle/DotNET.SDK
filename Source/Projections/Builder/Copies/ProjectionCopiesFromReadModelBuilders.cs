// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections.Builder.Copies;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopiesFromReadModelBuilders"/>.
/// </summary>
public class ProjectionCopiesFromReadModelBuilders : IProjectionCopiesFromReadModelBuilders
{
    readonly IEnumerable<ICanBuildCopyDefinitionFromReadModel> _augmenters;
    readonly ICreateCopiesDefinitionBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="IProjectionCopiesFromReadModelBuilders"/> class.
    /// </summary>
    /// <param name="augmenters">The <see cref="IEnumerable{T}"/> of <see cref="ICanBuildCopyDefinitionFromReadModel"/>.</param>
    /// <param name="builder">The <see cref="ICreateCopiesDefinitionBuilder"/>.</param>
    public ProjectionCopiesFromReadModelBuilders(IEnumerable<ICanBuildCopyDefinitionFromReadModel> augmenters, ICreateCopiesDefinitionBuilder builder)
    {
        _augmenters = augmenters;
        _builder = builder;
    }

    /// <inheritdoc />
    public bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, out ProjectionCopies projectionCopies)
        where TReadModel : class, new()
    {
        var builder = _builder.CreateFor<TReadModel>();
        projectionCopies = default;
        return BuildFrom(buildResults, builder) && builder.TryBuild(buildResults, out projectionCopies);
    }

    bool BuildFrom<TReadModel>(IClientBuildResults buildResults, IProjectionCopyDefinitionBuilder<TReadModel> builder)
        where TReadModel : class, new()
    {
        var succeeded = true;
        foreach (var augmenter in _augmenters.Where(_ => _.CanBuildFrom<TReadModel>()))
        {
            if (!augmenter.BuildFrom(buildResults, builder))
            {
                succeeded = false;
            }
        }
        if (succeeded)
        {
            return true;
        }
        buildResults.AddFailure($"Could not resolve projection copies definition for projection for read model type {typeof(TReadModel)}");
        return false;
    }
}
