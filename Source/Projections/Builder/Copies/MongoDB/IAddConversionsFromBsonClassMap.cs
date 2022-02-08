using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Defines a system that can add all conversions based from a <see cref="BsonClassMap"/>.
/// </summary>
public interface IAddConversionsFromBsonClassMap
{
    /// <summary>
    /// Adds all conversions to <see cref="IPropertyConversions"/>.
    /// </summary>
    /// <param name="classMap">The <see cref="BsonClassMap"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="conversions">The <see cref="IPropertyConversions"/>.</param>
    void AddFrom(BsonClassMap classMap, IClientBuildResults buildResults, IPropertyConversions conversions);
}