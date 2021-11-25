// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// 
    /// </summary>
    public class InstanceActivator : Autofac.Core.Activators.InstanceActivator, IInstanceActivator
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceActivator"/> class.
        /// </summary>
        /// <param name="limitType">The limit <see cref="Type"/>.</param>
        public InstanceActivator(Type limitType)
            : base(limitType)
        {
        }

        /// <inheritdoc />
        public void ConfigurePipeline(IComponentRegistryServices componentRegistryServices, IResolvePipelineBuilder pipelineBuilder)
        {
            if (pipelineBuilder is null)
            {
                throw new ArgumentNullException(nameof(pipelineBuilder));
            }

            pipelineBuilder.Use("whatever", PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (ctxt, next) =>
            {
                var wrapper = ctxt.Resolve<RootProviderWrapper>();
                var instance = wrapper
                    .ServiceProvider
                    .GetService(LimitType);
                if (instance != null)
                {
                    ctxt.Instance = instance;
                }

                // next(ctxt);
            });
        }
    }
}