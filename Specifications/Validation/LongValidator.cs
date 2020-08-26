// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;
using FluentValidation;

namespace Dolittle.Validation
{
    public class LongValidator : BusinessValidator<ConceptAs<long>>
    {
        public LongValidator()
        {
            RuleFor(c => c.Value)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
