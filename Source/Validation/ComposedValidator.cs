// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Dolittle.Validation
{
    /// <summary>
    /// Combines multiples validators into a single validator.
    /// </summary>
    /// <typeparam name="T">Type that the composite validator validates.</typeparam>
    public class ComposedValidator<T> : AbstractValidator<T>
    {
        readonly List<IValidator> registeredValidators = new List<IValidator>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedValidator{T}"/> class.
        /// </summary>
        /// <param name="validators"><see cref="IEnumerable{T}"/> of <see cref="IValidator"/> to compose.</param>
        public ComposedValidator(IEnumerable<IValidator> validators)
        {
            registeredValidators.AddRange(validators);
        }

        /// <summary>
        /// Creates a <see cref="IValidatorDescriptor"/> that can be used to obtain metadata about the current validator.
        /// </summary>
        /// <returns><see cref="IValidatorDescriptor"/> instance.</returns>
        public override IValidatorDescriptor CreateDescriptor()
        {
            return new ComposedValidatorDescriptor(registeredValidators.Select(m => m.CreateDescriptor()));
        }

        /// <summary>
        /// Validates the ValidationContext.
        /// </summary>
        /// <param name="context">Context of the validation.</param>
        /// <returns><see cref="FluentValidation.Results.ValidationResult"/>.</returns>
        public override FluentValidation.Results.ValidationResult Validate(ValidationContext<T> context)
        {
            var errors = registeredValidators.SelectMany(x => x.Validate(context).Errors);
            return new FluentValidation.Results.ValidationResult(errors);
        }
    }
}