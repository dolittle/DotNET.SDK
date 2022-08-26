// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned;

/// <summary>
/// Represents an implementation of <see cref="IPartitionedEventFilterBuilder"/>.
/// </summary>
public class PartitionedEventFilterBuilder : IPartitionedEventFilterBuilder, ICanBuildPrivateFilter
{
    PartitionedFilterEventCallback? _callback;

    /// <inheritdoc />
    public void Handle(PartitionedFilterEventCallback callback)
        => _callback = callback;

    /// <inheritdoc />
    public bool TryBuild(FilterModelId filterId, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_callback == default)
        {
            buildResults.AddError(new MissingFilterCallback(filterId));
            return false;
        }
        filter = new UnregisteredPartitionedEventFilter(filterId, _callback);
        return true;
    }
}
