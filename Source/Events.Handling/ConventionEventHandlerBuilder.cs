// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Events.Handling.Builder;
using Dolittle.Events.Handling.Internal;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Methods for building <see cref="IEventHandler{TEventType}"/> instances by convetion from implementations of <see cref="ICanHandle{TEventType}"/>.
    /// </summary>
    /// <typeparam name="TEventType">The type of events to handle.</typeparam>
    public static class ConventionEventHandlerBuilder<TEventType>
        where TEventType : IEvent
    {
        const string HandleMethodName = "Handle";

        /// <summary>
        /// Builds an <see cref="IEventHandler{TEventType}"/> from <typeparamref name="THandlerType"/> using a <see cref="FactoryFor{T}"/> to invoke the handler methods.
        /// </summary>
        /// <typeparam name="THandlerType">The type of the event handler to build for.</typeparam>
        /// <param name="factory">The <see cref="FactoryFor{T}"/> that will be used to instantiate the event handler.</param>
        /// <returns>A <see cref="IEventHandler{TEventType}"/> that can be registered with an <see cref="IRegisterEventHandlers"/>.</returns>
        public static IEventHandler<TEventType> BuildFor<THandlerType>(FactoryFor<THandlerType> factory)
        {
            var builder = EventHandlerBuilder.Create<TEventType>().With(factory);
            var handlerType = typeof(THandlerType);

            foreach (var method in handlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                ThrowIfMethodHasCorrectSignatureButWrongName(method);

                if (method.Name == HandleMethodName)
                {
                    ThrowIfFirstMethodParameterIsNot<TEventType>(method);
                    ThrowIfSecondMethodParameterIsNotEventContext(method);
                    ThrowIfMethodHasExtraParameters(method);
                    ThrowIfMethodDoesNotReturnATask(method);

                    TryGetFirstMethodParameter<TEventType>(method, out var eventType);
                    var delegateType = typeof(EventHandlerMethod<,>).MakeGenericType(handlerType, eventType);
                    var dynamicDelegate = method.CreateDelegate(delegateType, null);
                    var builderHandleMethod = typeof(EventHandlerFactoryBuilder<THandlerType, TEventType>).GetMethod(nameof(EventHandlerFactoryBuilder<THandlerType, TEventType>.Handle));
                    var dynamicHandleMethod = builderHandleMethod.MakeGenericMethod(eventType);
                    dynamicHandleMethod.Invoke(builder, new[] { dynamicDelegate });
                }
            }

            return builder.Build();
        }

        static void ThrowIfMethodHasCorrectSignatureButWrongName(MethodInfo method)
        {
            if (MethodHasHandleSignatureFor<IEvent>(method) && method.Name != HandleMethodName)
            {
                throw new EventHandlerMethodWithCorrectSignatureButWrongName(method, HandleMethodName);
            }
        }

        static void ThrowIfFirstMethodParameterIsNot<TTEventType>(MethodInfo method)
            where TTEventType : IEvent
        {
            if (!FirstMethodParameterIs<TEventType>(method))
            {
                throw new EventHandlerMethodFirstParameterMustBeCorrectEventType(typeof(TEventType), method);
            }
        }

        static void ThrowIfSecondMethodParameterIsNotEventContext(MethodInfo method)
        {
            if (!SecondMethodParameterIsEventContext(method))
            {
                throw new EventHandlerMethodSecondParameterMustBeEventContext(method);
            }
        }

        static void ThrowIfMethodHasExtraParameters(MethodInfo method)
        {
            if (!MethodHasNoExtraParameters(method))
            {
                throw new EventHandlerMethodMustTakeTwoParameters(method);
            }
        }

        static void ThrowIfMethodDoesNotReturnATask(MethodInfo method)
        {
            if (!MethodReturnsTask(method))
            {
                throw new EventHandlerMethodMustReturnATask(method);
            }
        }

        static bool MethodHasHandleSignatureFor<TTEventType>(MethodInfo method)
            where TTEventType : IEvent
            => FirstMethodParameterIs<TEventType>(method) && SecondMethodParameterIsEventContext(method) && MethodHasNoExtraParameters(method) && MethodReturnsTask(method);

        static bool FirstMethodParameterIs<TTEventType>(MethodInfo method)
            where TTEventType : IEvent
            => TryGetFirstMethodParameter<TTEventType>(method, out _);

        static bool TryGetFirstMethodParameter<TTEventType>(MethodInfo method, out Type type)
            where TTEventType : IEvent
        {
            type = null;
            if (method.GetParameters().Length > 0)
            {
                type = method.GetParameters()[0].ParameterType;
                return typeof(TTEventType).IsAssignableFrom(type);
            }

            return false;
        }

        static bool SecondMethodParameterIsEventContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EventContext);

        static bool MethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;

        static bool MethodReturnsTask(MethodInfo method)
            => method.ReturnType == typeof(Task);
    }
}