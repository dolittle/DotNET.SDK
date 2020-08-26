// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Filters.Internal;

namespace Dolittle.Events.Filters.EventHorizon
{
    /// <summary>
    /// Represents a system that is capable of providing implementations of <see cref="ICanFilterPublicEvents"/>.
    /// </summary>
    public interface ICanProvidePublicEventFilters : ICanProvideFilters<ICanFilterPublicEvents, IPublicEvent, PublicFilterResult>
    {
    }
}