// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Builder for building the <see cref="IOnMethod{TReadModel}"/> on a an embedding class.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
public class ClassProjectionMethodsBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
    where TEmbedding : class, new()
{
    const string ProjectionMethodName = "On";
    readonly IClientBuildResults _buildResults;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassProjectionMethodsBuilder{TEmbedding}"/> class.
    /// </summary>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public ClassProjectionMethodsBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, IClientBuildResults buildResults)
        : base(embeddingId, eventTypes)
    {
        _buildResults = buildResults;
    }

    /// <summary>
    /// Try to build an <see cref="IOnMethod{TEmbedding}"/> for each event type.
    /// </summary>
    /// <param name="eventTypesToMethods">A dictionary of event types and their respective projection methods.</param>
    /// <returns>A bool indicating whether the build succeeded.</returns>
    public bool TryBuild(out IDictionary<EventType, IOnMethod<TEmbedding>> eventTypesToMethods)
    {
        eventTypesToMethods = new Dictionary<EventType, IOnMethod<TEmbedding>>();
        var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
        var hasWrongMethods = !TryAddDecoratedProjectionMethodsInto(eventTypesToMethods, allMethods) || !TryAddConventionProjectionMethodsInto(eventTypesToMethods, allMethods);

        if (hasWrongMethods)
        {
            eventTypesToMethods = default;
            return false;
        }

        if (eventTypesToMethods.Count != 0)
        {
            return true;
        }
        _buildResults.AddFailure($"There are no projection methods to register in embedding {EmbeddingType}. A projection method either needs to be decorated with [{nameof(OnAttribute)}] or have the name {ProjectionMethodName}");
        eventTypesToMethods = default;
        return false;

    }

    bool TryAddDecoratedProjectionMethodsInto(IDictionary<EventType, IOnMethod<TEmbedding>> eventTypesToMethods, IEnumerable<MethodInfo> methods)
    {
        var allMethodsAdded = true;
        foreach (var method in methods.Where(IsDecoratedOnMethod))
        {
            var eventType = (method.GetCustomAttributes(typeof(OnAttribute), true)[0] as OnAttribute)?.EventType;

            var shouldAddHandler = TryGetEventParameterType(method, true, out var eventParameterType);
            shouldAddHandler = OnMethodSignatureAreOkay(method) && shouldAddHandler;
            shouldAddHandler = EvenParameterIsOfCorrectType(method, eventParameterType, true) && shouldAddHandler;
            shouldAddHandler = IsPublicMethod(method) && shouldAddHandler;

            if (!shouldAddHandler)
            {
                allMethodsAdded = false;
                continue;
            }

            if (eventTypesToMethods.TryAdd(eventType, CreateUntypedOnMethod(method, eventType)))
            {
                continue;
            }
            allMethodsAdded = false;
            _buildResults.AddFailure($"Event type {eventType} is already handled in projection {Embedding}");
        }

        return allMethodsAdded;
    }

    bool TryAddConventionProjectionMethodsInto(
        IDictionary<EventType, IOnMethod<TEmbedding>> eventTypesToMethods,
        IEnumerable<MethodInfo> methods)
    {
        var allMethodsAdded = true;
        foreach (var method in methods.Where(_ => !IsDecoratedOnMethod(_) && _.Name == ProjectionMethodName))
        {
            var shouldAddHandler = true;

            shouldAddHandler = TryGetEventParameterType(method, false, out var eventParameterType) && shouldAddHandler;
            shouldAddHandler = OnMethodSignatureAreOkay(method) && shouldAddHandler;
            shouldAddHandler = EvenParameterIsOfCorrectType(method, eventParameterType, false) && shouldAddHandler;
            shouldAddHandler = IsPublicMethod(method) && shouldAddHandler;

            if (!shouldAddHandler)
            {
                allMethodsAdded = false;
                continue;
            }

            var eventType = EventTypes.GetFor(eventParameterType);
            if (eventTypesToMethods.TryAdd(eventType, CreateTypedOnMethod(eventParameterType, method)))
            {
                continue;
            }
            allMethodsAdded = false;
            _buildResults.AddFailure($"Event type {eventParameterType} is already handled in embedding {Embedding}");
        }

        return allMethodsAdded;
    }

    IOnMethod<TEmbedding> CreateUntypedOnMethod(MethodInfo method, EventType eventType)
    {
        var onSignatureType = GetOnMethodSignature(method);
        var onSignature = method.CreateDelegate(onSignatureType.MakeGenericType(EmbeddingType), null);
        return Activator.CreateInstance(
            typeof(ClassOnMethod<>).MakeGenericType(EmbeddingType),
            onSignature,
            eventType) as IOnMethod<TEmbedding>;
    }

    IOnMethod<TEmbedding> CreateTypedOnMethod(Type eventParameterType, MethodInfo method)
    {
        var onSignatureGenericTypeDefinition = GetTypedOnMethodSignature(method);
        var onSignatureType = onSignatureGenericTypeDefinition.MakeGenericType(EmbeddingType, eventParameterType);
        var onSignature = method.CreateDelegate(onSignatureType, null);

        return Activator.CreateInstance(
            typeof(TypedClassOnMethod<,>).MakeGenericType(EmbeddingType, eventParameterType),
            onSignature) as IOnMethod<TEmbedding>;
    }

    Type GetOnMethodSignature(MethodInfo method)
    {
        if (MethodReturnsTask(method))
        {
            return typeof(TaskOnMethodSignature<>);
        }
        if (MethodReturnsTaskResultType(method))
        {
            return typeof(TaskResultOnMethodSignature<>);
        }
        if (MethodReturnsVoid(method))
        {
            return typeof(SyncOnMethodSignature<>);
        }
        if (MethodReturnsResultType(method))
        {
            return typeof(SyncResultOnMethodSignature<>);
        }
        throw new InvalidProjectionMethodReturnType(method.ReturnType);
    }

    Type GetTypedOnMethodSignature(MethodInfo method)
    {
        if (MethodReturnsTask(method))
        {
            return typeof(TaskOnMethodSignature<,>);
        }
        if (MethodReturnsTaskResultType(method))
        {
            return typeof(TaskResultOnMethodSignature<,>);
        }
        if (MethodReturnsVoid(method))
        {
            return typeof(SyncOnMethodSignature<,>);
        }
        if (MethodReturnsResultType(method))
        {
            return typeof(SyncResultOnMethodSignature<,>);
        }
        throw new InvalidProjectionMethodReturnType(method.ReturnType);
    }

    bool OnMethodSignatureAreOkay(MethodInfo method)
    {
        var okay = true;
        if (!SecondMethodParameterIsEmbeddingProjectionContext(method))
        {
            okay = false;
            _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} needs to have two parameters where the second parameter is {typeof(EmbeddingProjectContext)}");
        }

        if (!ProjectionMethodHasNoExtraParameters(method))
        {
            okay = false;
            _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} needs to only have two parameters where the first is the event to handle and the second is {typeof(EmbeddingProjectContext)}");
        }

        if (!MethodReturnsAsyncVoid(method) && (MethodReturnsVoid(method) || MethodReturnsResultType(method) || MethodReturnsTask(method) || MethodReturnsTaskResultType(method)))
        {
            return okay;
        }
        _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} needs to return either {typeof(void)}, {typeof(ProjectionResultType)}, {typeof(Task)}, {typeof(Task<ProjectionResultType>)}");

        return false;
    }

    bool TryGetEventParameterType(MethodInfo method, bool isDecorated, out Type type)
    {
        type = default;
        if (method.GetParameters().Length == 0)
        {
            _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} has no parameters{(isDecorated ? ", but is decorated with [{nameof(OnAttribute)}]" : string.Empty)}. A projection method should take in as parameters an event and an {nameof(EmbeddingProjectContext)}");
            return false;
        }

        type = method.GetParameters()[0].ParameterType;
        return true;
    }

    bool EvenParameterIsOfCorrectType(MethodInfo method, Type eventParameterType, bool isDecorated)
    {
        if (isDecorated && eventParameterType != typeof(object))
        {
            _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} should only handle an event of type object");
            return false;
        }

        if (eventParameterType == typeof(object))
        {
            _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} cannot handle an 'object' event when not decorated with [{nameof(OnAttribute)}]");
            return false;
        }

        if (EventTypes.HasFor(eventParameterType))
        {
            return true;
        }
        _buildResults.AddFailure($"Projection method {method} on embedding {EmbeddingType} handles event of type {eventParameterType}, but it is not associated to any event type");
        return false;

    }

    bool IsPublicMethod(MethodInfo method)
    {
        if (method.IsPublic)
        {
            return true;
        }
        _buildResults.AddFailure($"Method {method} on embedding {EmbeddingType} has the signature of an projection method, but is not public. Projection methods needs to be public");
        return false;

    }

    static bool IsDecoratedOnMethod(MethodInfo method)
        => method.GetCustomAttributes(typeof(OnAttribute), true).FirstOrDefault() != default;

    static bool SecondMethodParameterIsEmbeddingProjectionContext(MethodInfo method)
        => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingProjectContext);

    static bool ProjectionMethodHasNoExtraParameters(MethodInfo method)
        => method.GetParameters().Length == 2;

    static bool MethodReturnsTaskResultType(MethodInfo method)
        => method.ReturnType == typeof(Task<ProjectionResultType>);

    static bool MethodReturnsResultType(MethodInfo method)
        => method.ReturnType == typeof(ProjectionResultType);
}
