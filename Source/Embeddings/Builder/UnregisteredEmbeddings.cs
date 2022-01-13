// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Common;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEmbeddings"/>.
/// </summary>
public class UnregisteredEmbeddings : UniqueBindings<EmbeddingId, Internal.IEmbedding>, IUnregisteredEmbeddings
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEmbeddings"/> class.
    /// </summary>
    /// <param name="eventHandlers">The unique <see cref="IEmbedding"/> projections.</param>
    /// <param name="readModelTypes">The <see cref="IEmbeddingReadModelTypes"/>.</param>
    public UnregisteredEmbeddings(IUniqueBindings<EmbeddingId, Internal.IEmbedding> eventHandlers, IEmbeddingReadModelTypes readModelTypes)
        : base(eventHandlers)
    {
        ReadModelTypes = readModelTypes;
    }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IConvertEventsToProtobuf eventsConverter,
        IConvertProjectionsToSDK projectionsConverter,
        IEventTypes eventTypes,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken)
    {
        foreach (var projection in Values)
        {
            eventProcessors.Register(
                CreateProjectionsProcessor(
                    projection,
                    eventsConverter,
                    projectionsConverter,
                    eventTypes,
                    loggerFactory),
                new EmbeddingsProtocol(),
                cancelConnectToken,
                stopProcessingToken);
        }
    }

    /// <inheritdoc />
    public IEmbeddingReadModelTypes ReadModelTypes { get; }

    static EventProcessor<EmbeddingId, EmbeddingRegistrationRequest, EmbeddingRequest, EmbeddingResponse> CreateProjectionsProcessor(
        Internal.IEmbedding embedding,
        IConvertEventsToProtobuf eventsConverter,
        IConvertProjectionsToSDK projectionConverter,
        IEventTypes eventTypes,
        ILoggerFactory loggerFactory)
    {
        var processorType = typeof(EmbeddingsProcessor<>).MakeGenericType(embedding.ReadModelType); 
        return Activator.CreateInstance(
            processorType,
            embedding,
            eventsConverter,
            projectionConverter,
            eventTypes,
            loggerFactory.CreateLogger(processorType)) as EventProcessor<EmbeddingId, EmbeddingRegistrationRequest, EmbeddingRequest, EmbeddingResponse>;
    }
}
