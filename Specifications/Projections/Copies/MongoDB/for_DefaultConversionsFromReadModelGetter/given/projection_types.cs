// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_DefaultConversionsFromReadModelGetter.given;

public class projection_type_without_fields
{
    
}

public class projection_type_with_fields_without_bson_type_attributes
{
    public int Integer { get; set; }
    public long Long { get; set; }
    public decimal Decimal { get; set; }
    public double Double { get; set; }
    public string String { get; set; }
    public byte[] Binary { get; set; }
    public bool Boolean { get; set; }
    public DateTime DateTime { get; set; }
    public Timestamp TimeStamp { get; set; }
}