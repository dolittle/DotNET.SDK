// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Provides bindings.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IRegisterEventHandlers>().To<EventHandlerRegistry>();
        }
    }
}
