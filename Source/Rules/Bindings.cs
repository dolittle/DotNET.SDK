// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Types;

namespace Dolittle.Rules
{
    /// <summary>
    /// Represents the bindings for working with rules.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        readonly ITypeFinder _typeFinder;
        readonly GetContainer _getContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bindings"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for discovering rules.</param>
        /// <param name="getContainer"><see cref="GetContainer"/> for getting the <see cref="IContainer"/>.</param>
        public Bindings(ITypeFinder typeFinder, GetContainer getContainer)
        {
            _typeFinder = typeFinder;
            _getContainer = getContainer;
        }

        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            var interfaceType = typeof(IRuleImplementationFor<>);
            var ruleImplementations = _typeFinder.FindMultiple(interfaceType);
            ruleImplementations.ForEach(_ =>
            {
                var @interface = _.GetInterfaces().SingleOrDefault(t => $"{t.Namespace}.{t.Name}" == $"{interfaceType.Namespace}.{interfaceType.Name}");
                var ruleType = @interface.GetGenericArguments()[0];

                builder.Bind(ruleType).To(() =>
                {
                    var ruleImplementation = _getContainer().Get(_);
                    var ruleProperty = _.GetProperty("Rule", BindingFlags.Public | BindingFlags.Instance);
                    return ruleProperty.GetValue(ruleImplementation);
                });
            });
        }
    }
}