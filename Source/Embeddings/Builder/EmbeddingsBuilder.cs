// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
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
        var identifier = new EmbeddingModelId(embeddingId, "");
        var builder = new EmbeddingBuilder(identifier, _modelBuilder);
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildEmbedding, EmbeddingModelId, EmbeddingId>(identifier, builder);
        return builder;
    }

    /// <inheritdoc />
    public IEmbeddingsBuilder Register<TEmbedding>()
        where TEmbedding : class, new()
        => Register(typeof(TEmbedding));

    /// <inheritdoc />
    public IEmbeddingsBuilder Register(Type type)
    {
        if (!_decoratedTypeBindings.TryAdd(type, out var binding))
        {
            return this;
        }
        BindBuilder(binding);
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingsBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEmbeddingBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var binding in addedEmbeddingBindings)
        {
            BindBuilder(binding);
        }

        return this;
    }
    void BindBuilder(TypeBinding<EmbeddingModelId, EmbeddingId> binding)
        =>  _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildEmbedding, EmbeddingModelId, EmbeddingId>(
            binding.Identifier,
            CreateConventionEmbeddingBuilderFor(binding));
    
    static ICanTryBuildEmbedding CreateConventionEmbeddingBuilderFor(TypeBinding<EmbeddingModelId, EmbeddingId> binding)
        => Activator.CreateInstance(
                typeof(ConventionEmbeddingBuilder<>).MakeGenericType(binding.Type))
            as ICanTryBuildEmbedding;

    /// <summary>
    /// Build embeddings.
    /// </summary>
    /// <param name="applicationModel">The <see cref="IApplicationModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    public static IUnregisteredEmbeddings Build(IApplicationModel applicationModel, IEventTypes eventTypes, IClientBuildResults buildResults)
    {
        var embeddings = new UniqueBindings<EmbeddingModelId, Internal.IEmbedding>();
        foreach (var (identifier, builder) in applicationModel.GetProcessorBuilderBindings<ICanTryBuildEmbedding, EmbeddingModelId, EmbeddingId>())
        {
            if (builder.TryBuild(identifier, eventTypes, buildResults, out var embedding))
            {
                embeddings.Add(identifier, embedding);
            }
        }
        var identifiers = applicationModel.GetTypeBindings<EmbeddingModelId, EmbeddingId>();
        var readModelTypes = new EmbeddingReadModelTypes();
        foreach (var (identifier, type) in identifiers)
        {
            readModelTypes.Add(identifier.Id, type);
        }
        return new UnregisteredEmbeddings(embeddings, readModelTypes);
    }
}
