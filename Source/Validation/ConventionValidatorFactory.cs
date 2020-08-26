// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.DependencyInversion;
using FluentValidation;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents a <see cref="IValidatorFactory"/> that is based on conventions.
    /// </summary>
    public class ConventionValidatorFactory : IValidatorFactory
    {
        readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionValidatorFactory"/> class.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> to use for getting instances of <see cref="IValidator">validators</see>.</param>
        public ConventionValidatorFactory(IContainer container)
        {
            _container = container;
        }

        /// <inheritdoc/>
        public IValidator<T> GetValidator<T>()
        {
            var type = typeof(T);
            var validatorTypeName = $"{type.Name}Validator";
            var validatorType = type.GetTypeInfo().Assembly.GetType(validatorTypeName);
            return _container.Get(validatorType) as IValidator<T>;
        }

        /// <inheritdoc/>
        public IValidator GetValidator(Type type)
        {
            var validatorTypeName = $"{type.Namespace}.{type.Name}Validator";
            var validatorType = type.GetTypeInfo().Assembly.GetType(validatorTypeName);
            if (validatorType != null)
            {
                return _container.Get(validatorType) as IValidator;
            }

            return null;
        }
    }
}
