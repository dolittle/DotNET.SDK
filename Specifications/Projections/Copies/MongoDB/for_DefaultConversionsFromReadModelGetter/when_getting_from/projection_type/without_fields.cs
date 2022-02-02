// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_DefaultConversionsFromReadModelGetter.when_getting_from.projection_type;

public class without_fields : given.a_resolver
{
    
    Because of = () => conversions_result = FromReadModelGetter.GetFrom<given.projection_type_without_fields>();
    
    It should_have_no_conversions = () => conversions_result.ShouldBeEmpty();
}