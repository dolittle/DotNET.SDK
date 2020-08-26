// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Dolittle.Validation.for_BusinessValidator.given
{
    public class ChildValidator : BusinessValidator<Child>
    {
        public ChildValidator()
        {
            RuleFor(c => c.ChildConcept)
                .NotNull()
                .SetValidator(new ConceptAsLongValidator());
            RuleFor(c => c.ChildSimpleStringProperty)
                .NotEmpty();
            RuleFor(c => c.ChildSimpleIntegerProperty)
                .LessThan(10);
            RuleFor(c => c.Grandchild)
                .SetValidator(new GrandchildValidator());
        }
    }
}