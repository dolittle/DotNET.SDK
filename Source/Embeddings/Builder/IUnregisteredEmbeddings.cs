// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Defines a collection of unregistered projections.
/// </summary>
public interface IUnregisteredEmbeddings : IUniqueBindings<EmbeddingModelId, Internal.IEmbedding>
{
    /// <summary>
    /// Registers projections.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
    /// <param name="eventsConverter">The <see cref="IConvertEventsToProtobuf" />.</param>
    /// <param name="projectionsConverter">The <see cref="IConvertProjectionsToSDK"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    /// <param name="cancelConnectToken">The <see cref="CancellationToken" />.</param>
    /// <param name="stopProcessingToken">The <see cref="CancellationToken" /> for stopping processing.</param>
    void Register(
        IEventProcessors eventProcessors,
        IConvertEventsToProtobuf eventsConverter,
        IConvertProjectionsToSDK projectionsConverter,
        IEventTypes eventTypes,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken);
    
    /// <summary>
    /// Gets the <see cref="IEmbeddingReadModelTypes"/>.
    /// </summary>
    IEmbeddingReadModelTypes ReadModelTypes { get; }
}
