// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Dolittle.Validation.for_BusinessValidator
{
    public class ValidatorWithModelRuleWithOneMustClause : BusinessValidator<SimpleObject>
    {
        public bool CallbackCalled = false;

        public ValidatorWithModelRuleWithOneMustClause()
        {
            ModelRule().Must(o => CallbackCalled = true);
        }
    }
}
