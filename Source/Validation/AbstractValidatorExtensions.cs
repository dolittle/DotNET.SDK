﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.Reflection;
using FluentValidation;
using FluentValidation.Internal;

namespace Dolittle.Validation
{
    /// <summary>
    /// Validation extensions for building validation rules.
    /// </summary>
    public static class AbstractValidatorExtensions
    {
        /// <summary>
        /// Defines a concept validation rule for a specify property.
        /// </summary>
        /// <typeparam name="T">Type to add validation rules for.</typeparam>
        /// <typeparam name="TProperty">The type of property being validated.</typeparam>
        /// <param name="validator">The validator to apply the validation rule to.</param>
        /// <param name="expression">The expression representing the property to validate.</param>
        /// <returns>an IRuleBuilder instance on which validators can be defined.</returns>
        public static IRuleBuilderInitial<T, TProperty> AddRuleForConcept<T, TProperty>(
            this AbstractValidator<T> validator,
            Expression<Func<T, TProperty>> expression)
        {
            var rule = PropertyRule.Create(expression, () => validator.CascadeMode);
            rule.PropertyName = FromExpression(expression) ?? typeof(TProperty).Name;
            validator.AddRule(rule);
            return new RuleBuilder<T, TProperty>(rule);
        }

        static string FromExpression(LambdaExpression expression)
        {
            var stack = new Stack<string>();
            for (var memberExpression = ExpressionExtensions.Unwrap(expression.Body);
                memberExpression != null;
                memberExpression = ExpressionExtensions.Unwrap(memberExpression.Expression))
            {
                stack.Push(memberExpression.Member.Name);
            }

            return string.Join(".", stack.ToArray());
        }
    }
}
