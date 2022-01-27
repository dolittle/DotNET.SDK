// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;
using PbProjectionCopies = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopies;

namespace Dolittle.SDK.Projections.Copies;

public record ProjectionCopies(ProjectionCopyToMongoDB MongoDB)
{
    public PbProjectionCopies ToProtobuf()
    {
        var result = new PbProjectionCopies
        {
            MongoDB = MongoDB.ToProtobuf()
        };
        return result;
    }
}

