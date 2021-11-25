// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK
{
    public class CustomControllerActivator : IControllerActivator
    {
        readonly Func<Type, ObjectFactory> _createFactory =
            (type) => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
        readonly ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache =
            new ConcurrentDictionary<Type, ObjectFactory>();
        readonly Func<IContainer> _containerFactory;

        public CustomControllerActivator(IDolittleClient dolittleClient)
        {
            _containerFactory = () => dolittleClient.Services;
        }

        /// <inheritdoc />
        public object Create(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.ActionDescriptor == null)
            {
                throw new ArgumentException("ControllerContext.ActionDescriptor cannot be null");
            }
            var controllerTypeInfo = context.ActionDescriptor.ControllerTypeInfo;

            if (controllerTypeInfo == null)
            {
                throw new ArgumentException("ControllerContext.ActionDescriptor.ControllerTypeInfo cannot be null");
            }
        
            var tenantHeader = context.HttpContext.Request.Headers["Tenant-Id"];
            var tenantId = new TenantId { Value = Guid.Parse(tenantHeader) };
            var serviceProvider = _containerFactory().GetProviderFor(tenantId);
            context.HttpContext.RequestServices = serviceProvider;

            return CreateInstance<object>(serviceProvider, controllerTypeInfo.AsType());
        }

        /// <inheritdoc />
        public void Release(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public ValueTask ReleaseAsync(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (controller is IAsyncDisposable asyncDisposable)
            {
                return asyncDisposable.DisposeAsync();
            }

            Release(context, controller);
            return default;
        }

        TInstance CreateInstance<TInstance>(IServiceProvider serviceProvider, Type implementationType)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            var createFactory = _typeActivatorCache.GetOrAdd(implementationType, _createFactory);
            return (TInstance)createFactory(serviceProvider, arguments: null);
        }
    }
}