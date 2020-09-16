// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing.Internal;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Defines a filter processor.
    /// </summary>
    /// <typeparam name="TRegisterRequest">The <see cref="System.Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
    public interface IFilterProcessor<TRegisterRequest, TResponse> : IEventProcessor<FilterId, TRegisterRequest, FilterEventRequest, TResponse>
        where TRegisterRequest : class
        where TResponse : class
    {
    }
}