// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Dolittle.Validation.for_BusinessValidator.given
{
    public class GrandchildValidator : BusinessValidator<Grandchild>
    {
        public GrandchildValidator()
        {
            RuleFor(gc => gc.GrandchildConcept)
                .NotNull()
                .SetValidator(new ConceptAsLongValidator());
            RuleFor(gc => gc.GrandchildSimpleStringProperty)
                .NotEmpty();
            RuleFor(gc => gc.GrandchildSimpleIntegerProperty)
                .LessThan(10);
        }
    }
}