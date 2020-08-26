// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.DependencyInversion;

namespace Dolittle.Heads
{
    /// <summary>
    /// Provides bindings related to client.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            var head = new Head(Guid.NewGuid());
            builder.Bind<Head>().To(head);
        }
    }
}