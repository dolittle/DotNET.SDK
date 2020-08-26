// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Commands.Validation;
using Dolittle.DependencyInversion;
using Dolittle.Reflection;
using Dolittle.Types;
using FluentValidation;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents the default <see cref="IValidatorFactory"/> implementation used for validators.
    /// </summary>
    public class DefaultValidatorFactory : IValidatorFactory
    {
        readonly ITypeFinder _typeFinder;
        readonly IContainer _container;
        readonly Dictionary<Type, Type> _validatorsByType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidatorFactory"/> class.
        /// </summary>
        /// <param name="typeFinder">A <see cref="ITypeFinder"/> used for discovering validators.</param>
        /// <param name="container">A <see cref="IContainer"/> to use for creating instances of the different validators.</param>
        public DefaultValidatorFactory(ITypeFinder typeFinder, IContainer container)
        {
            _container = container;
            _validatorsByType = new Dictionary<Type, Type>();
            _typeFinder = typeFinder;
            Populate();
        }

        /// <inheritdoc/>
        public IValidator GetValidator(Type type)
        {
            if (!_validatorsByType.ContainsKey(type))
                return null;

            var instance = _container.Get(_validatorsByType[type]) as IValidator;
            return instance;
        }

        /// <inheritdoc/>
        public IValidator<T> GetValidator<T>()
        {
            return GetValidator(typeof(T)) as IValidator<T>;
        }

        void Populate()
        {
            var validatorTypes = _typeFinder.FindMultiple(typeof(IValidator)).Where(
                t =>
                    !t.HasInterface<ICommandInputValidator>() &&
                    !t.HasInterface<ICommandBusinessValidator>());
            foreach (var validatorType in validatorTypes)
            {
                var genericArguments = validatorType.GetTypeInfo().BaseType.GetTypeInfo().GetGenericArguments();
                if (genericArguments.Length == 1)
                {
                    var targetType = genericArguments[0];
                    _validatorsByType[targetType] = validatorType;
                }
            }
        }
    }
}
#endif