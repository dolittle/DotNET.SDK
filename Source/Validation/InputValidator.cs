// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using FluentValidation;

namespace Dolittle.Validation
{
    /// <summary>
    /// Base class to inherit from for basic input validation rules.
    /// </summary>
    /// <typeparam name="T">Type to add validation rules for.</typeparam>
    public class InputValidator<T> : AbstractValidator<T>, IValidateInput<T>
    {
        /// <summary>
        /// Validates the provided instance using the specified ruleset.
        /// </summary>
        /// <param name="instance">TThe object to be validated.</param>
        /// <param name="ruleSet">A comma separated list of rulesets to be used when validating.</param>
        /// <returns>A ValidationResult object containing any validation failures.</returns>
        public FluentValidation.Results.ValidationResult Validate(T instance, string ruleSet)
        {
            return (this as IValidator<T>).Validate(instance, ruleSet: ruleSet);
        }

        /// <summary>
        /// Defines a concept validation rule for a specify property.
        /// </summary>
        /// <example>
        /// RuleForConcept(x => x.Surname)...
        /// </example>
        /// <typeparam name="TProperty">The type of property being validated.</typeparam>
        /// <param name="expression">The expression representing the property to validate.</param>
        /// <returns>an IRuleBuilder instance on which validators can be defined.</returns>
        public IRuleBuilderInitial<T, TProperty> RuleForConcept<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return this.AddRuleForConcept(expression);
        }
    }
}