// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_DefaultConversionsFromReadModelGetter.given;

public class a_resolver
{
    protected static DefaultConversionsFromReadModelGetter FromReadModelGetter;
    protected static ClientBuildResults build_results;
    protected static IDictionary<ProjectionField, Conversion> conversions_result;
    Establish context = () =>
    {
        build_results = new ClientBuildResults();
        FromReadModelGetter = new DefaultConversionsFromReadModelGetter();
    };
}