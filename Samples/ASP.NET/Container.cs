// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Execution;
using Microsoft.Extensions.DependencyInjection;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace ASP.NET
{
    public class Container : IContainer
    {
        private static AsyncLocal<ExecutionContext> _currentExecutionContext = new AsyncLocal<ExecutionContext>();
        public static ExecutionContext CurrentExecutionContext => _currentExecutionContext.Value;
        
        
        readonly  IServiceProvider _provider;

        public Container(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object Get(Type service, ExecutionContext context)
        {
            _currentExecutionContext.Value = context;
            return _provider.GetService(service);
        }

        public T Get<T>(ExecutionContext context)
            where T : class
        {
            _currentExecutionContext.Value = context;
            return _provider.GetService<T>();
        }
    }
}