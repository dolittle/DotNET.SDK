// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Builder for building <see cref="ClassUpdateMethod{TEmbedding}"/>.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
public class ClassUpdateMethodBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
    where TEmbedding : class, new()
{
    const string UpdateMethodName = "ResolveUpdateToEvents";
    readonly IClientBuildResults _buildResults;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassUpdateMethodBuilder{TEmbedding}"/> class.
    /// </summary>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public ClassUpdateMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, IClientBuildResults buildResults)
        : base(embeddingId, eventTypes)
    {
        _buildResults = buildResults;
    }

    /// <summary>
    /// Try to build an <see cref="IUpdateMethod{TEmbedding}"/>.
    /// </summary>
    /// <param name="method">The out of the method.</param>
    /// <returns>A bool indicating whether the build succeeded.</returns>
    public bool TryBuild(out IUpdateMethod<TEmbedding> method)
    {
        var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
        if (TryAddDecoratedUpdateMethod(allMethods, out method) || TryAddConventionUpdateMethod(allMethods, out method))
        {
            return true;
        }
        _buildResults.AddFailure($"No update method defined for embedding {EmbeddingType} with id {Embedding}. An embedding needs to have one update method, which is either named {UpdateMethodName} or attributed with [{nameof(ResolveUpdateToEventsAttribute)}].");
        return false;

    }

    bool TryAddDecoratedUpdateMethod(MethodInfo[] allMethods, out IUpdateMethod<TEmbedding> updateMethod)
    {
        updateMethod = default;
        if (allMethods.Count(IsDecoratedUpdateMethod) > 1)
        {
            _buildResults.AddFailure($"More than one update method attributed on embedding {EmbeddingType} with id {Embedding}. An embedding can only have one update method.");
            return false;
        }

        var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedUpdateMethod);
        if (!UpdateMethodIsOkay(decoratedMethod))
        {
            return false;
        }

        updateMethod = CreateUpdateMethod(decoratedMethod);
        return true;
    }

    bool TryAddConventionUpdateMethod(MethodInfo[] allMethods, out IUpdateMethod<TEmbedding> updateMethod)
    {
        updateMethod = default;

        if (allMethods.Count(_ => IsDecoratedUpdateMethod(_) || _.Name == UpdateMethodName) > 1)
        {
            _buildResults.AddFailure($"More than one update method on embedding {EmbeddingType} with id {Embedding}. An embedding can only have one update method called {UpdateMethodName} or attributed with [{nameof(ResolveUpdateToEventsAttribute)}].");
            return false;
        }

        var conventionMethod = allMethods
            .SingleOrDefault(_ => !IsDecoratedUpdateMethod(_) && _.Name == UpdateMethodName);
        if (!UpdateMethodIsOkay(conventionMethod))
        {
            return false;
        }

        updateMethod = CreateUpdateMethod(conventionMethod);
        return true;
    }

    bool UpdateMethodIsOkay(MethodInfo method)
    {
        if (method == default || !UpdateMethodParametersAreOkay(method))
        {
            return false;
        }

        if (!MethodReturnsTaskOrVoid(method))
        {
            return true;
        }
        _buildResults.AddFailure($"Update method {method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.");
        return false;
    }

    bool UpdateMethodParametersAreOkay(MethodInfo method)
    {
        var okay = true;
        if (!SecondMethodParameterIsEmbeddingContext(method))
        {
            okay = false;
            _buildResults.AddFailure($"Update method {method} on embedding {EmbeddingType} needs to have two parameters, where the second parameters is {typeof(EmbeddingContext)}");
        }

        if (UpdateMethodHasNoExtraParameters(method))
        {
            return okay;
        }

        _buildResults.AddFailure($"Update method {method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {typeof(EmbeddingContext)}");
        return false;
    }

    IUpdateMethod<TEmbedding> CreateUpdateMethod(MethodInfo method)
    {
        var updateSignatureType = GetUpdateSignatureType(method);
        var updateSignature = method.CreateDelegate(updateSignatureType.MakeGenericType(EmbeddingType), null);
        return Activator.CreateInstance(
            typeof(ClassUpdateMethod<>).MakeGenericType(EmbeddingType),
            updateSignature) as IUpdateMethod<TEmbedding>;
    }

    Type GetUpdateSignatureType(MethodInfo method)
    {
        if (MethodReturnsTaskOrVoid(method))
        {
            throw new InvalidUpdateMethodReturnType(method.ReturnType);
        }

        return MethodReturnsEnumerableObject(method) ?
            typeof(UpdateMethodEnumerableReturnSignature<>)
            : typeof(UpdateMethodSignature<>);
    }

    static bool SecondMethodParameterIsEmbeddingContext(MethodInfo method)
        => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingContext);

    static bool IsDecoratedUpdateMethod(MethodInfo method)
        => method.GetCustomAttributes(typeof(ResolveUpdateToEventsAttribute), true).FirstOrDefault() != default;

    static bool UpdateMethodHasNoExtraParameters(MethodInfo method)
        => method.GetParameters().Length == 2;
}
