// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.when_getting_for_type;

public class and_there_is_no_definition : given.no_associations
{
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => artifacts.GetFor(typeof(some_class)));

    It should_fail_because_of_no_artifact_associated_with_type = () => exception.ShouldBeOfExactType<MissingUniqueBindingForValue<given.artifact_type, Type>>();
}