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
    /// Gets the <see cref="FilterModelId"/>.
    /// </summary>
    FilterModelId Identifier { get; }

    /// <summary>
    /// Register the event filter processor.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEventProcessors"/>.</param>
    /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter converter,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken);
}
