// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Represents the builder for configuring embeddings.
/// </summary>
public class EmbeddingsBuilder : IEmbeddingsBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly DecoratedTypeBindingsToModelAdder<EmbeddingAttribute, EmbeddingModelId, EmbeddingId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingsBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public EmbeddingsBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _modelBuilder = modelBuilder;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<EmbeddingAttribute, EmbeddingModelId, EmbeddingId>("embeddings", modelBuilder, buildResults);
    }

    /// <inheritdoc />
    public IEmbeddingBuilder Create(EmbeddingId embeddingId)
    {
        var builder = new EmbeddingBuilder(embeddingId, _modelBuilder);
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildEmbedding>(new EmbeddingModelId(embeddingId, null), builder);
        return builder;
    }

    /// <inheritdoc />
    public IEmbeddingsBuilder Register<TEmbedding>()
        where TEmbedding : class, new()
        => Register(typeof(TEmbedding));

    /// <inheritdoc />
    public IEmbeddingsBuilder Register(Type type)
    {
        if (!_decoratedTypeBindings.TryAdd(type, out var decorator))
        {
            return this;
        }
        BindBuilder(type, decorator);
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingsBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEmbeddingBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var (type, decorator) in addedEmbeddingBindings)
        {
            BindBuilder(type, decorator);
        }

        return this;
    }
    void BindBuilder(Type type, EmbeddingModelId identifier)
        =>  _modelBuilder.BindIdentifierToProcessorBuilder(
            identifier,
            CreateConventionEmbeddingBuilderFor(type, identifier));
    
    static ICanTryBuildEmbedding CreateConventionEmbeddingBuilderFor(Type type, EmbeddingModelId identifier)
        => Activator.CreateInstance(
                typeof(ConventionEmbeddingBuilder<>).MakeGenericType(type),
                identifier)
            as ICanTryBuildEmbedding;

    /// <summary>
    /// Build embeddings.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    public static IUnregisteredEmbeddings Build(IModel model, IEventTypes eventTypes, IClientBuildResults buildResults)
    {
        var embeddings = new UniqueBindings<EmbeddingModelId, Internal.IEmbedding>();
        foreach (var builder in model.GetProcessorBuilderBindings<ICanTryBuildEmbedding>().Select(_ => _.ProcessorBuilder))
        {
            if (builder.TryBuild(eventTypes, buildResults, out var embedding))
            {
                embeddings.Add(new EmbeddingModelId(embedding.Identifier, null), embedding);
            }
        }
        var identifiers = model.GetTypeBindings<EmbeddingModelId, EmbeddingId>();
        var readModelTypes = new EmbeddingReadModelTypes();
        foreach (var (identifier, type) in identifiers)
        {
            readModelTypes.Add(identifier.Id, type);
        }
        return new UnregisteredEmbeddings(embeddings, readModelTypes);
    }
}
