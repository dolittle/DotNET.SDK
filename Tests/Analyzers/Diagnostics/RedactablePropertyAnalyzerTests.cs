// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class RedactablePropertyAnalyzerTests : AnalyzerTest<RedactablePropertyAnalyzer>
{
    [Fact]
    public async Task OnPropertyWithoutRedactableAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    public string SomeProperty { get; set; }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(code);
    }
    
    [Fact]
    public async Task OnPropertyWithNullableRedactableAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    [RedactablePersonalData]
    public string? SomeProperty { get; set; }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(code);
    }
    
    [Fact]
    public async Task OnPropertyWithReplacedRedactableStringAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    [RedactablePersonalData<string>(""<removed>"")]
    public string SomeProperty { get; set; }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(code);
    }
    
    [Fact]
    public async Task OnPropertyWithReplacedRedactableStringAttributeWithDifferentNullability()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    [RedactablePersonalData<string>(""<removed>"")]
    public string? SomeProperty { get; set; }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(code);
    }
    
    [Fact]
    public async Task OnPropertyWithReplacedRedactableIntAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    [RedactablePersonalData<int>(-1)]
    public int SomeProperty { get; set; }
}
";
        
        await VerifyAnalyzerFindsNothingAsync(code);
    }
    
    [Fact]
    public async Task OnPropertyWithNonNullableRedactableAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    [RedactablePersonalData]
    public string SomeProperty { get; set; }
}
";
        await VerifyAnalyzerAsync(code,
            Diagnostic(DescriptorRules.NonNullableRedactableField)
                .WithSpan(8, 5, 9, 45).WithArguments("SomeProperty"));
    }
    
    [Fact]
    public async Task OnPropertyWithIntAttributeOnString()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
class SomeEvent
{
    [RedactablePersonalData<int>(123)]
    public string SomeProperty { get; set; }
}
";
        await VerifyAnalyzerAsync(code,
            Diagnostic(DescriptorRules.IncorrectRedactableFieldType)
                .WithSpan(8, 6, 8, 38).WithMessage("The generic type for RedactablePersonalDataAttribute was Int32, must match the property type String"));
    }
    
    [Fact]
    public async Task OnRecordPropertyWithNonNullableRedactableAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
record SomeEvent([property: RedactablePersonalData] string SomeProperty);
";
        await VerifyAnalyzerAsync(code,
            Diagnostic(DescriptorRules.NonNullableRedactableField)
                .WithSpan(6, 18, 6, 72).WithArguments("SomeProperty"));
    }
    
    [Fact]
    public async Task OnRecordPropertyWithNullableRedactableAttribute()
    {
        var code = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

[EventType(""44a755a7-e755-4076-bad4-fbc6ec3c0ea5"")]
record SomeEvent([property: RedactablePersonalData] string? SomeProperty);
";
        await VerifyAnalyzerFindsNothingAsync(code);
    }
}
