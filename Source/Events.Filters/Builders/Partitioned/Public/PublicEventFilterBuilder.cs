// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned.Public;

/// <summary>
/// Represents the builder for building public event filters.
/// </summary>
public class PublicEventFilterBuilder : IPartitionedEventFilterBuilder
{
    PartitionedFilterEventCallback _callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicEventFilterBuilder"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    public PublicEventFilterBuilder(FilterId filterId) => FilterId = filterId;

    /// <summary>
    /// Gets the <see cref="FilterId" /> of the filter that this builder builds.
    /// </summary>
    public FilterId FilterId { get; }


    /// <inheritdoc />
    public void Handle(PartitionedFilterEventCallback callback)
        => _callback = callback;

    /// <summary>
    /// Builds the public event filter.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="filter">The outputted <see cref="ICanRegisterEventFilterProcessor"/> that can register the public event filter.</param>
    /// <returns>A value indicating whether the building succeeded or not.</returns>
    public bool TryBuild(IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_callback == default)
        {
            buildResults.AddError(new MissingFilterCallback(FilterId, ScopeId.Default));
            return false;
        }
        filter = new UnregisteredPublicEventFilter(FilterId, _callback);
        return true;
    }
}
