// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Defines a <see cref="ICanBuildPropertyConversionsFromReadModel"/> that builds property conversions from projection properties marked with <see cref="MongoDBConvertToAttribute"/>. 
/// </summary>
public interface IBuildPropertyConversionsFromMongoDBConvertToAttributes : ICanBuildPropertyConversionsFromReadModel
{
}
