// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Builder for building <see cref="ClassDeleteMethod{TEmbedding}"/>.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
public class ClassDeleteMethodBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
    where TEmbedding : class, new()
{
    const string DeleteMethodName = "ResolveDeletionToEvents";
    readonly IClientBuildResults _buildResults;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassDeleteMethodBuilder{TEmbedding}"/> class.
    /// </summary>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public ClassDeleteMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, IClientBuildResults buildResults)
        : base(embeddingId, eventTypes)
    {
        _buildResults = buildResults;
    }

    /// <summary>
    /// Try to build an <see cref="IDeleteMethod{TEmbedding}"/>.
    /// </summary>
    /// <param name="method">The out of the method.</param>
    /// <returns>A bool indicating whether the build succeeded.</returns>
    public bool TryBuild(out IDeleteMethod<TEmbedding> method)
    {
        var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
        if (TryAddDecoratedDeleteMethod(allMethods, out method) || TryAddConventionDeleteMethod(allMethods, out method))
        {
            return true;
        }
        _buildResults.AddFailure($"No deletion method defined for embedding {EmbeddingType} with id {Embedding}. An embedding needs to have one deletion method, which is either named {DeleteMethodName} or attributed with [{nameof(ResolveDeletionToEventsAttribute)}].");
        return false;

    }

    bool TryAddDecoratedDeleteMethod(
        MethodInfo[] allMethods,
        out IDeleteMethod<TEmbedding> deleteMethod)
    {
        deleteMethod = default;
        if (allMethods.Count(IsDecoratedDeleteMethod) > 1)
        {
            _buildResults.AddFailure($"More than one Delete method attributed on embedding {EmbeddingType} with id {Embedding}. An embedding can only have one Delete method.");
            return false;
        }

        var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedDeleteMethod);
        if (!DeleteMethodIsOkay(decoratedMethod))
        {
            return false;
        }

        deleteMethod = CreateDeleteMethod(decoratedMethod);
        return true;
    }

    bool TryAddConventionDeleteMethod(
        MethodInfo[] allMethods,
        out IDeleteMethod<TEmbedding> deleteMethod)
    {
        deleteMethod = default;

        if (allMethods.Count(_ => IsDecoratedDeleteMethod(_) || _.Name == DeleteMethodName) > 1)
        {
            _buildResults.AddFailure($"More than one deletion method on embedding {EmbeddingType} with id {Embedding}. An embedding can only have one deletion method called {DeleteMethodName} or attributed with [{nameof(ResolveDeletionToEventsAttribute)}].");
            return false;
        }

        var conventionMethod = allMethods
            .SingleOrDefault(_ => !IsDecoratedDeleteMethod(_) && _.Name == DeleteMethodName);
        if (!DeleteMethodIsOkay(conventionMethod))
        {
            return false;
        }

        deleteMethod = CreateDeleteMethod(conventionMethod);
        return true;
    }

    bool DeleteMethodIsOkay(MethodInfo method)
    {
        if (method == default || !DeleteMethodParametersAreOkay(method))
        {
            return false;
        }

        if (!MethodReturnsTaskOrVoid(method))
        {
            return true;
        }
        _buildResults.AddFailure($"Deletion method {method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.");
        return false;

    }

    bool DeleteMethodParametersAreOkay(MethodInfo method)
    {
        var okay = true;
        if (!FirstMethodParameterIsEmbeddingContext(method))
        {
            okay = false;
            _buildResults.AddFailure($"Deletion method {method} on embedding {EmbeddingType} needs to have only one {typeof(EmbeddingContext)} parameter");
        }

        if (!DeleteMethodHasNoExtraParameters(method))
        {
            okay = false;
            _buildResults.AddFailure($"Deletion method {method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {typeof(EmbeddingContext)}");
        }

        if (!MethodReturnsTaskOrVoid(method))
        {
            return okay;
        }
        _buildResults.AddFailure($"Deletion method {method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.");
        return false;

    }

    IDeleteMethod<TEmbedding> CreateDeleteMethod(MethodInfo method)
    {
        var deleteSignatureType = GetDeleteSignatureType(method);
        var deleteSignature = method.CreateDelegate(deleteSignatureType.MakeGenericType(EmbeddingType), null);
        return Activator.CreateInstance(
            typeof(ClassDeleteMethod<>).MakeGenericType(EmbeddingType),
            deleteSignature) as IDeleteMethod<TEmbedding>;
    }

    Type GetDeleteSignatureType(MethodInfo method)
    {
        if (MethodReturnsTaskOrVoid(method))
        {
            throw new InvalidDeleteMethodReturnType(method.ReturnType);
        }

        return MethodReturnsEnumerableObject(method)
            ? typeof(DeleteMethodEnumerableReturnSignature<>)
            : typeof(DeleteMethodSignature<>);
    }

    static bool FirstMethodParameterIsEmbeddingContext(MethodInfo method)
        => method.GetParameters().Length > 0 && method.GetParameters()[0].ParameterType == typeof(EmbeddingContext);

    static bool IsDecoratedDeleteMethod(MethodInfo method)
        => method.GetCustomAttributes(typeof(ResolveDeletionToEventsAttribute), true).FirstOrDefault() != default;

    static bool DeleteMethodHasNoExtraParameters(MethodInfo method)
        => method.GetParameters().Length == 1;
}
