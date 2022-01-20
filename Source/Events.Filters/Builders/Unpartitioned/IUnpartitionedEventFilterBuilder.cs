// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.Filters.Builders.Unpartitioned;

/// <summary>
/// Defines a builder for building unpartitioned event filters.
/// </summary>
public interface IUnpartitionedEventFilterBuilder
{
    /// <summary>
    /// Defines a callback for the filter.
    /// </summary>
    /// <param name="callback">The callback that will be called for each event.</param>
    void Handle(FilterEventCallback callback);
}
