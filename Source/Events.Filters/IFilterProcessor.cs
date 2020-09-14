// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing.Internal;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Defines a filter processor.
    /// </summary>
    public interface IFilterProcessor : IEventProcessor<FilterRegistrationResponse>
    {
    }
}