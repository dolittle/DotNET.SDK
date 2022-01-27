// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.when_augmenting.given;

public class all_dependencies : for_MongoDBCopyAdder.given.all_dependencies
{
    protected static ProjectionCopies projection_copies;
    protected static ProjectionCopies augmented_result;
    protected static bool succeeded;

    Establish context = () =>
    {
        projection_copies = new ProjectionCopies(null);
    };
}