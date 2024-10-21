// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class RedactionEventTests : AnalyzerTest<AttributeIdentityAnalyzer>
{
    [Fact]
    public async Task WhenHasCorrectPrefix()
    {
        var test = @"

[Dolittle.SDK.Events.EventType(""e8879da9-fd28-4c78-b9cc-1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
};

[Dolittle.SDK.Events.EventType(""de1e7e17-bad5-da7a-aaaa-fbc6ec3c0ea6"")]
class RedactionEvent: Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<SomeEvent>
{
}
";

        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task WhenHasCorrectPrefixAlternateFormatting()
    {
        var test = @"

[Dolittle.SDK.Events.EventType(""e8879da9fd284c78b9cc1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
};

[Dolittle.SDK.Events.EventType(""de1e7e17bad5da7aaaaafbc6ec3c0ea6"")]
class RedactionEvent: Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<SomeEvent>
{
}
";

        await VerifyAnalyzerFindsNothingAsync(test);
    }

    [Fact]
    public async Task WhenHasInCorrectPrefixButValidGuid()
    {
        var test = @"
[Dolittle.SDK.Events.EventType(""e8879da9-fd28-4c78-b9cc-1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
};

[Dolittle.SDK.Events.EventType(""de1e7e17-0000-da7a-aaaa-fbc6ec3c0ea6"")]
class RedactionEvent: Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<SomeEvent>
{
}
";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.IncorrectRedactedEventTypePrefix)
                .WithSpan(8, 2, 8, 71)
                .WithArguments("de1e7e17-0000-da7a-aaaa-fbc6ec3c0ea6"),
        };

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task WhenHasInCorrectPrefixAndNotAGuid()
    {
        var test = @"
[Dolittle.SDK.Events.EventType(""e8879da9-fd28-4c78-b9cc-1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
};

[Dolittle.SDK.Events.EventType(""hello-0000-da7a-aaaa-fbc6ec3c0ea6"")]
class RedactionEvent: Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<SomeEvent>
{
}
";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.IncorrectRedactedEventTypePrefix)
                .WithSpan(8, 2, 8, 68)
                .WithArguments("hello-0000-da7a-aaaa-fbc6ec3c0ea6"),
        };

        await VerifyAnalyzerAsync(test, expected);
    }

}
