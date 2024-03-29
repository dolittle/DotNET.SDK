// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Exception that gets thrown when a sequence of events are not valid for the Aggregate Root it is being used with.
/// </summary>s
public class AggregateRootVersionIsOutOfOrder : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootVersionIsOutOfOrder"/> class.
    /// </summary>
    /// <param name="eventVersion">The <see cref="AggregateRootVersion"/> the Event was applied by.</param>
    /// <param name="previousVersion">Previous <see cref="AggregateRootVersion"/>.</param>
    public AggregateRootVersionIsOutOfOrder(AggregateRootVersion eventVersion, AggregateRootVersion previousVersion)
        : base($"Aggregate Root version is out of order. Version '{eventVersion}' from event is not greater than previous version '{previousVersion}'")
    {
    }
}
