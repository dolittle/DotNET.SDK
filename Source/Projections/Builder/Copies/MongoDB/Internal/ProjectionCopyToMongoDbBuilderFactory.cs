// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using Dolittle.SDK.Projections.Copies;
// using Dolittle.SDK.Projections.Copies.MongoDB;
//
// namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
//
// /// <summary>
// /// Represents an implementation of <see cref="IProjectionCopyToMongoDBBuilderFactory"/>.
// /// </summary>
// public class ProjectionCopyToMongoDbBuilderFactory : IProjectionCopyToMongoDBBuilderFactory
// {
//     readonly IBuildPropertyConversionsFromBsonClassMap _conversionsFromBsonClassMapBuilder;
//     readonly IBuildPropertyConversionsFromMongoDBConvertToAttributes _conversionsFromAttributesBuilder;
//     readonly IResolvePropertyPath _propertyPathResolver;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="ProjectionCopyToMongoDbBuilderFactory"/> class.
//     /// </summary>
//     /// <param name="collectionNameValidator"></param>
//     /// <param name="conversionsFromBsonClassMapBuilder"></param>
//     /// <param name="conversionsFromAttributesBuilder"></param>
//     /// <param name="propertyPathResolver">The <see cref="IResolvePropertyPath"/>.</param>
//     public ProjectionCopyToMongoDbBuilderFactory(
//         IBuildPropertyConversionsFromBsonClassMap conversionsFromBsonClassMapBuilder,
//         IBuildPropertyConversionsFromMongoDBConvertToAttributes conversionsFromAttributesBuilder,
//         IResolvePropertyPath propertyPathResolver)
//     {
//         _conversionsFromBsonClassMapBuilder = conversionsFromBsonClassMapBuilder;
//         _conversionsFromAttributesBuilder = conversionsFromAttributesBuilder;
//         _propertyPathResolver = propertyPathResolver;
//     }
//     
//     /// <inheritdoc />
//     public IProjectionCopyToMongoDBBuilder<TReadModel> CreateFor<TReadModel>() where TReadModel : class, new()
//         => new ProjectionCopyToMongoDBBuilder<TReadModel>(_conversionsFromBsonClassMapBuilder, new MongoDBCopyDefinitionFromReadModelBuilder(_conversionsFromAttributesBuilder), _propertyPathResolver);
// }
