// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_PropertyPathResolver.when_resolving_from_expression;

public class calling_method : given.all_dependencies
{
    static Exception exception;
    Because of = () => exception = Catch.Exception(() => resolve_from_expression(_ => _.Method()));

    It should_fail = () => exception.ShouldBeOfExactType<ArgumentException>();
}