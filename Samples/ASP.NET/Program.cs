// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using Dolittle.SDK.Extensions.AspNet;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDolittle();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseDolittle();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
//
// var cm = BsonClassMap.LookupClassMap(typeof(projection_type_with_fields_without_bson_type_attributes));
// var x = cm.Discriminator;
//
// public class projection_type_with_fields_without_bson_type_attributes
// {
//     public int Integer { get; set; }
//     public long Long { get; set; }
//     public decimal Decimal { get; set; }
//     public double Double { get; set; }
//     public string String { get; set; }
//     public byte[] Binary { get; set; }
//     public bool Boolean { get; set; }
//     public DateTime DateTime { get; set; }
//     public Timestamp TimeStamp { get; set; }
//     public DateTimeOffset DateTimeOffset { get; set; }
// }
