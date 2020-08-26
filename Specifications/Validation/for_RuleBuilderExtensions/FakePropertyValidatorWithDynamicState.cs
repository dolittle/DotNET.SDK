// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using FluentValidation.Validators;

namespace Dolittle.Validation.for_RuleBuilderExtensions
{
    public class FakePropertyValidatorWithDynamicState : PropertyValidatorWithDynamicState
    {
        public FakePropertyValidatorWithDynamicState()
            : base("")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return true;
        }

        public bool AddExpressionCalled = false;

        public override void AddExpression<T>(Expression<Func<T, object>> expression)
        {
            AddExpressionCalled = true;
        }
    }
}
