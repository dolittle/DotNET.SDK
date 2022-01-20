// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Represents an implementation of <see cref="IEmbeddings" />.
/// </summary>
public class Embeddings : IEmbeddings
{
    readonly IPerformMethodCalls _caller;
    readonly IResolveCallContext _callContextResolver;
    readonly IEmbeddingReadModelTypes _embeddingAssociations;
    readonly IConvertProjectionsToSDK _projectionsToSDKConverter;
    readonly ExecutionContext _executionContext;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Embeddings"/> class.
    /// </summary>
    /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
    /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
    /// <param name="embeddingAssociations">The <see cref="IEmbeddingReadModelTypes" />.</param>
    /// <param name="projectionsToSDKConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public Embeddings(
        IPerformMethodCalls caller,
        IResolveCallContext callContextResolver,
        IEmbeddingReadModelTypes embeddingAssociations,
        IConvertProjectionsToSDK projectionsToSDKConverter,
        ExecutionContext executionContext,
        ILoggerFactory loggerFactory)
    {
        _caller = caller;
        _callContextResolver = callContextResolver;
        _embeddingAssociations = embeddingAssociations;
        _projectionsToSDKConverter = projectionsToSDKConverter;
        _executionContext = executionContext;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public IEmbedding ForTenant(TenantId tenant)
    {
        var executionContext = _executionContext
            .ForTenant(tenant)
            .ForCorrelation(Guid.NewGuid());
        return new Embedding(
            _caller,
            _callContextResolver,
            executionContext,
            _embeddingAssociations,
            _projectionsToSDKConverter,
            _loggerFactory);
    }
}