// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class EventsHaveAnnotationAnalyzerTests : AnalyzerTest<EventsHaveAnnotationAnalyzer>
{
    [Fact]
    public async Task ShouldDetectNoIssues()
    {
        const string test = @"
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{

}

public class Committer
{
    readonly Dolittle.SDK.Events.Store.ICommitEvents _committer;

    public Committer(ICommitEvents committer)
    {
        _committer = committer;
    }

    public async Task CommitValidThing()
    {
        await _committer.CommitEvent(new SomeEvent(), """", CancellationToken.None);
    }
}
";


        await VerifyAnalyzerFindsNothingAsync(test);
    }

    [Fact]
    public async Task ShouldDetectNoIssuesWhenCommittingObject()
    {
        const string test = @"
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store;

public class Committer
{
    readonly Dolittle.SDK.Events.Store.ICommitEvents _committer;

    public Committer(ICommitEvents committer)
    {
        _committer = committer;
    }

    public async Task CommitValidThing(object @event)
    {
        await _committer.CommitEvent(@event, """", CancellationToken.None);
    }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldDetectNoIssuesWhenCommittingSystemObject()
    {
        const string test = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store;

public class Committer
{
    readonly Dolittle.SDK.Events.Store.ICommitEvents _committer;

    public Committer(ICommitEvents committer)
    {
        _committer = committer;
    }

    public async Task CommitValidThing(Object @event)
    {
        await _committer.CommitEvent(@event, """", CancellationToken.None);
    }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldDetectEventTypeWithUsing()
    {
        const string test = @"
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store;

class SomeInvalidEvent
{
    
}

public class Committer
{
    readonly Dolittle.SDK.Events.Store.ICommitEvents _committer;

    public Committer(ICommitEvents committer)
    {
        _committer = committer;
    }

    public async Task CommitInvalidThing()
    {
        await _committer.CommitEvent(new SomeInvalidEvent(), """", CancellationToken.None);
    }
}
";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(22, 15, 22, 89)
                .WithArguments("SomeInvalidEvent")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldDetectMissingAttributeWhenUsedWithClient()
    {
        const string test = @"
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;

class SomeInvalidEvent
{
    
}

public class ClientTest
{
    private IDolittleClient _client;

    public ClientTest(IDolittleClient client)
    {
        _client = client;
    }

    public async Task DoThing()
    {
        var @event = new SomeInvalidEvent();
        var eventStore = _client.EventStore.ForTenant(TenantId.System);
        await eventStore.CommitEvent(@event, """", CancellationToken.None);
    }
}
";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(26, 15, 26, 73)
                .WithArguments("SomeInvalidEvent")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
}
