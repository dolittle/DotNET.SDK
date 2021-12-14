// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Defines a builder that can build a private filter.
/// </summary>
public interface ICanRegisterEventFilterProcessor
{
    /// <summary>
    /// Register the event filter processor.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEventProcessors"/>.</param>
    /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    /// <param name="cancelConnectToken">The <see cref="CancellationToken" />.</param>
    /// <param name="stopProcessingToken">The <see cref="CancellationToken" /> for stopping processing.</param>
    void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter converter,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken);
}
