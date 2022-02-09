// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;
using PbProjectionCopyToMongoDB = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB;
namespace Dolittle.SDK.Projections.Copies.MongoDB.for_ProjectionCopyToMongoDB;

public class when_converting_to_protobuf
{
    static ProjectionCopyToMongoDB sdk_instance;
    static PropertyConversion sdk_conversion;
    static PbProjectionCopyToMongoDB protobuf_instance;

    Establish context = () =>
    {
        sdk_conversion = new PropertyConversion("property", Conversion.GuidAsString, Enumerable.Empty<PropertyConversion>(), "rename");
        sdk_instance = new ProjectionCopyToMongoDB(
            true,
            "name",
            new[] { sdk_conversion });
    };
    Because of = () => protobuf_instance = sdk_instance.ToProtobuf();

    It should_have_the_correct_collection_name = () => protobuf_instance.Collection.ShouldEqual(sdk_instance.CollectionName.Value);
    It should_have_one_conversion = () => protobuf_instance.Conversions.Count.ShouldEqual(1);
    It should_have_conversion_with_correct_property_name = () => protobuf_instance.Conversions.First().PropertyName.ShouldEqual(sdk_conversion.PropertyName.Value);
    It should_have_conversion_with_correct_property_conversion = () => protobuf_instance.Conversions.First().ConvertTo.ShouldEqual(PbProjectionCopyToMongoDB.Types.BSONType.GuidasString);
    It should_have_conversion_with_correct_property_renaming = () => protobuf_instance.Conversions.First().RenameTo.ShouldEqual(sdk_conversion.RenameTo.Value);
    It should_have_conversion_with_no_child_conversions = () => protobuf_instance.Conversions.First().Children.ShouldBeEmpty();
}