// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_ProjectionCopyToMongoDB;

public class when_getting_default
{
    static ProjectionCopyToMongoDB default_instance;

    Because of = () => default_instance = ProjectionCopyToMongoDB.Default;

    It should_not_copy = () => default_instance.ShouldCopy.ShouldBeFalse();
}