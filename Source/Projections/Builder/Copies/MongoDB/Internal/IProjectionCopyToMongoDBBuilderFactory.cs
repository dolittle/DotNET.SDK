// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System;
//
// namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
//
// /// <summary>
// /// Defines a factory for <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
// /// </summary>
// public interface IProjectionCopyToMongoDBBuilderFactory
// {
//     /// <summary>
//     /// Creates <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/> for a specific <typeparamref name="TReadModel"/>.
//     /// </summary>
//     /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
//     /// <returns>The created <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/> for a specific <typeparamref name="TReadModel"/>.</returns>
//     IProjectionCopyToMongoDBBuilder<TReadModel> CreateFor<TReadModel>()
//         where TReadModel : class, new();
// }
