// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using Dolittle.SDK.Projections.Copies.MongoDB;
// using PbProjectionCopies = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopies;
//
// namespace Dolittle.SDK.Projections.Copies;
//
// /// <summary>
// /// Represents the projection copies definition.
// /// </summary>
// /// <param name="MongoDB">The <see cref="ProjectionCopyToMongoDB" />.</param>
// public record ProjectionCopies(ProjectionCopyToMongoDB MongoDB)
// {
//     /// <summary>
//     /// Converts the <see cref="ProjectionCopies"/> to <see cref="PbProjectionCopies"/>.
//     /// </summary>
//     /// <returns>The <see cref="PbProjectionCopies"/>.</returns>
//     public PbProjectionCopies ToProtobuf()
//     {
//         var result = new PbProjectionCopies();
//         if (MongoDB.ShouldCopy)
//         {
//             result.MongoDB = MongoDB.ToProtobuf();
//         }
//         return result;
//     }
// }
//
