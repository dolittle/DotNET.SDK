// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Filters.Builders.Unpartitioned;

/// <summary>
/// Represents the builder for building unpartitioned event filters.
/// </summary>
public class UnpartitionedEventFilterBuilder : IUnpartitionedEventFilterBuilder, ICanBuildPrivateFilter
{
    FilterEventCallback _callback;
    
    /// <inheritdoc />
    public void Handle(FilterEventCallback callback)
        => _callback = callback;
    
    /// <inheritdoc />
    public bool TryBuild(FilterId filterId, ScopeId scopeId, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_callback == default)
        {
            buildResults.AddError(new MissingFilterCallback(filterId, scopeId));
            return false;
        }
        filter = new UnregisteredUnpartitionedEventFilter(filterId, scopeId, _callback);
        return true;
    }
}
