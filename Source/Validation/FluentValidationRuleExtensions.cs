// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using FluentValidation.Validators;

namespace Dolittle.Validation
{
    /// <summary>
    /// Extensions for FluentValidation rules.
    /// </summary>
    public static class FluentValidationRuleExtensions
    {
        /// <summary>
        /// Start building a dynamic validation rule.
        /// </summary>
        /// <typeparam name="T">Type being validated.</typeparam>
        /// <typeparam name="TProperty">Type of property being validated.</typeparam>
        /// <param name="ruleBuilder"><see cref="IRuleBuilder{T,TProperty}"/> being extended.</param>
        /// <param name="validator"><see cref="IValidator"/> to add.</param>
        /// <param name="name">Display name.</param>
        /// <returns><see cref="IRuleBuilderOptions{T, TProperty}"/> for building.</returns>
        public static IRuleBuilderOptions<T, TProperty> DynamicValidationRule<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            IValidator validator,
            string name)
        {
            var builder = ruleBuilder
                    .NotNull()
                    .WithName(name);

            if (validator is IPropertyValidator)
            {
                builder = builder.SetValidator(validator as IPropertyValidator);
            }

            return builder;
        }
    }
}
