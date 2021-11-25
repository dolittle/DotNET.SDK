// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;

var client = await DolittleClient
    .Setup(_ => {})
    .Connect().ConfigureAwait(false);

var committedEvent = await client.EventStore
    .ForTenant(TenantId.Development)
    .Commit(_ => _.CreateEvent(new SomeEvent()).FromEventSource("d38c78e4-37d4-4549-80f4-ef9ca2af08d3").WithEventType("d38c78e4-37d4-4549-80f4-ef9ca2af08d3"))
    .ConfigureAwait(false);

Console.WriteLine($"Committed event with sequence number {committedEvent.First().EventLogSequenceNumber}");

record SomeEvent(string message = "Hello");