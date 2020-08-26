// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Validation.for_RuleBuilderExtensions
{
    public class when_adding_dynamic_state_to_validator
    {
        static Mock<AbstractValidator<object>> validator_mock;
        static FakePropertyValidatorWithDynamicState property_validator;

        Establish context = () =>
        {
            validator_mock = new Mock<AbstractValidator<object>>();
            property_validator = new FakePropertyValidatorWithDynamicState();
        };

        Because of = () => validator_mock.Object.RuleFor(o => o).SetValidator(property_validator).WithDynamicStateFrom(o => o);

        It should_add_expression_to_validaotr = () => property_validator.AddExpressionCalled.ShouldBeTrue();
    }
}
