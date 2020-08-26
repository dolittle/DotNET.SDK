// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;

namespace Dolittle.Validation
{
    /// <summary>
    /// Validation extensions for building validation rules.
    /// </summary>
    public static class RuleBuilderExtensions
    {
        /// <summary>
        /// Start building dynamic state for a rule.
        /// </summary>
        /// <typeparam name="T">Type being validated.</typeparam>
        /// <typeparam name="TProperty">Type of property being validated.</typeparam>
        /// <param name="builder"><see cref="IRuleBuilderOptions{T,TProperty}"/> being extended.</param>
        /// <param name="expression">Expression for setting state.</param>
        /// <returns>Chained <see cref="IRuleBuilderOptions{T, TProperty}"/> for building.</returns>
        public static IRuleBuilderOptions<T, TProperty> WithDynamicStateFrom<T, TProperty>(this IRuleBuilderOptions<T, TProperty> builder, Expression<Func<T, object>> expression)
        {
            ThrowIfNotCorrectRuleBuilder(builder);
            ThrowIfNotCorrectValidator(builder);

            var validator = ((RuleBuilder<T, TProperty>)builder).Rule.CurrentValidator as PropertyValidatorWithDynamicState;
            validator.AddExpression(expression);
            return builder;
        }

        static void ThrowIfNotCorrectValidator<T, TProperty>(IRuleBuilderOptions<T, TProperty> builder)
        {
            var actualBuilder = builder as RuleBuilder<T, TProperty>;
            if (!(actualBuilder.Rule.CurrentValidator is PropertyValidatorWithDynamicState))
            {
                throw new InvalidValidatorType();
            }
        }

        static void ThrowIfNotCorrectRuleBuilder<T, TProperty>(IRuleBuilderOptions<T, TProperty> builder)
        {
            if (!(builder is RuleBuilder<T, TProperty>))
                throw new InvalidRuleBuilderType(builder.GetType());
        }
    }
}
