// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Threading.Tasks;
//
// namespace Dolittle.SDK.Analyzers.CodeFixes;
//
// public class EventAnnotationMissingInAggregateCodeFixTests : CodeFixProviderTests<AggregateAnalyzer, AttributeMissingCodeFixProvider>
// { 
//     //Line ending Roslyn bugs break the test: https://github.com/dotnet/roslyn/issues/62976
//     
//     [Fact]
//     public async Task FixAttributeWithMissingIdentity()
//     {
//         var test = @"
// using Dolittle.SDK.Aggregates;
// using Dolittle.SDK.Events;
//
// namespace Test;
//
// record NameUpdated(string Name);
//
// [AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeAggregate: AggregateRoot
// {
//     public string Name {get; set;}
//
//     private void On(NameUpdated @event)
//     {
//         Name = @event.Name;
//     }
// }";
//
//         var expected = @"
// using Dolittle.SDK.Aggregates;
// using Dolittle.SDK.Events;
//
// namespace Test;
//
// [EventType(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
// record NameUpdated(string Name);
//
// [AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeAggregate: AggregateRoot
// {
//     public string Name {get; set;}
//
//     private void On(NameUpdated @event)
//     {
//         Name = @event.Name;
//     }
// }";
//         IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
//         var diagnosticResult = Diagnostic(DescriptorRules.Events.MissingAttribute)
//             .WithSpan(14, 21, 14, 39)
//             .WithArguments("Test.NameUpdated");
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
//     
//     [Fact]
//     public async Task FixAttributeWithMissingIdentityAndNamespace()
//     {
//         var test = @"
// using Dolittle.SDK.Aggregates;
//
// namespace Test;
//
// record NameUpdated(string Name);
//
// [AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeAggregate: AggregateRoot
// {
//     public string Name {get; set;}
//
//     private void On(NameUpdated @event)
//     {
//         Name = @event.Name;
//     }
// }";
//
//         var expected = @"using Dolittle.SDK.Aggregates;
// using Dolittle.SDK.Events;
//
// namespace Test;
// [EventType(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
// record NameUpdated(string Name);
// [AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeAggregate : AggregateRoot
// {
//     public string Name { get; set; }
//
//     private void On(NameUpdated @event)
//     {
//         Name = @event.Name;
//     }
// }";
//         IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
//         var diagnosticResult = Diagnostic(DescriptorRules.Events.MissingAttribute)
//             .WithSpan(13, 21, 13, 39)
//             .WithArguments("Test.NameUpdated");
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
//
// }
