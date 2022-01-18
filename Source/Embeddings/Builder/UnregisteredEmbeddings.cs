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
public class UnregisteredEmbeddings : UniqueBindings<EmbeddingModelId, Internal.IEmbedding>, IUnregisteredEmbeddings
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEmbeddings"/> class.
    /// </summary>
    /// <param name="embeddings">The unique <see cref="IEmbedding"/> embeddings.</param>
    /// <param name="readModelTypes">The <see cref="IEmbeddingReadModelTypes"/>.</param>
    public UnregisteredEmbeddings(IUniqueBindings<EmbeddingModelId, Internal.IEmbedding> embeddings, IEmbeddingReadModelTypes readModelTypes)
        : base(embeddings)
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
        CancellationToken cancellationToken)
    {
        foreach (var embedding in Values)
        {
            eventProcessors.Register(
                CreateEmbeddingsProcessor(
                    embedding,
                    eventsConverter,
                    projectionsConverter,
                    eventTypes,
                    loggerFactory),
                new EmbeddingsProtocol(),
                cancellationToken);
        }
    }

    /// <inheritdoc />
    public IEmbeddingReadModelTypes ReadModelTypes { get; }

    static EventProcessor<EmbeddingId, EmbeddingRegistrationRequest, EmbeddingRequest, EmbeddingResponse> CreateEmbeddingsProcessor(
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
