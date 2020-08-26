// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Dolittle.Validation
{
    public class ObjectValidator : BusinessValidator<object>
    {
        public ObjectValidator()
        {
            ModelRule()
                .NotNull();
        }
    }
}
